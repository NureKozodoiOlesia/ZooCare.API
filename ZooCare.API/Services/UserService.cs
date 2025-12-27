using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public UserService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();

            return users.Select(u => new UserDto(
                u.Id,
                u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                u.UserRoles.Select(ur => ur.Role!.Name).ToList()
            ));
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            return new UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.UserRoles.Select(ur => ur.Role!.Name).ToList()
            );
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // Перевірка чи користувач вже існує
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.UserName == dto.UserName);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Користувач з таким email або username вже існує");
            }

            // Хешування пароля
            var passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return new UserDto(
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                new List<string>()
            );
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з ID {id} не знайдений");
            }

            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                user.FirstName = dto.FirstName;
            }

            if (!string.IsNullOrEmpty(dto.LastName))
            {
                user.LastName = dto.LastName;
            }

            if (!string.IsNullOrEmpty(dto.Email))
            {
                // Перевірка чи email не зайнятий іншим користувачем
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);

                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email вже використовується іншим користувачем");
                }

                user.Email = dto.Email;
                user.UserName = dto.Email;
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            // Отримуємо оновлені дані з ролями
            return (await GetUserByIdAsync(id))!;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveAsync();

            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

