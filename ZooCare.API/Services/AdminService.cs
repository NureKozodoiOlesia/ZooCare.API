using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public AdminService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles.Select(r => new RoleDto(r.Id, r.Name, r.NormalizedName));
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
            {
                return null;
            }
            return new RoleDto(role.Id, role.Name, role.NormalizedName);
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
        {
            // Перевірка чи роль вже існує
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == dto.Name.ToUpper());

            if (existingRole != null)
            {
                throw new InvalidOperationException($"Роль з назвою '{dto.Name}' вже існує");
            }

            var role = new Role
            {
                Name = dto.Name,
                NormalizedName = dto.Name.ToUpper()
            };

            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveAsync();

            return new RoleDto(role.Id, role.Name, role.NormalizedName);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            // Перевірка чи роль використовується
            var userRolesCount = await _context.UserRoles
                .CountAsync(ur => ur.RoleId == id);

            if (userRolesCount > 0)
            {
                throw new InvalidOperationException("Неможливо видалити роль, яка призначена користувачам");
            }

            _unitOfWork.Roles.Delete(role);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> InitializeRolesAsync()
        {
            var roles = new[]
            {
                new { Name = "Адміністратор", NormalizedName = "ADMIN" },
                new { Name = "Доглядач", NormalizedName = "CARETAKER" },
                new { Name = "Ветеринар", NormalizedName = "VETERINARIAN" }
            };

            foreach (var roleData in roles)
            {
                var existingRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.NormalizedName == roleData.NormalizedName);

                if (existingRole == null)
                {
                    var role = new Role
                    {
                        Name = roleData.Name,
                        NormalizedName = roleData.NormalizedName
                    };
                    await _unitOfWork.Roles.AddAsync(role);
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, string roleName)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper());

            if (role == null)
            {
                return false;
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

            if (userRole == null)
            {
                return false; // Роль не призначена
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role!.Name)
                .ToListAsync();

            return userRoles;
        }

        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.PasswordHash = HashPassword(newPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<AdminStatsDto> GetSystemStatsAsync()
        {
            var stats = new AdminStatsDto(
                TotalUsers: await _context.Users.CountAsync(),
                TotalAnimals: await _context.Animals.CountAsync(),
                TotalEnclosures: await _context.Enclosures.CountAsync(),
                TotalTasks: await _context.DailyTasks.CountAsync(),
                PendingTasks: await _context.DailyTasks.CountAsync(t => t.Status == "Pending"),
                CompletedTasks: await _context.DailyTasks.CountAsync(t => t.Status == "Done"),
                ActiveAlerts: await _context.SystemAlerts.CountAsync(a => !a.IsResolved),
                TotalAlerts: await _context.SystemAlerts.CountAsync(),
                OnlineDevices: await _context.IoTDevices.CountAsync(d => d.Status == "Online"),
                TotalDevices: await _context.IoTDevices.CountAsync()
            );

            return stats;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

