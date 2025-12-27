using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public AuthService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            // Перевірка чи користувач вже існує
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.UserName == dto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Користувач з таким email вже існує");
            }

            // Хешування пароля
            var passwordHash = HashPassword(dto.Password);

            // Створення користувача
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            // Призначення ролі "Доглядач" за замовчуванням
            var caretakerRole = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == "CARETAKER");
            if (caretakerRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = caretakerRole.Id
                };
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }

            // Генерація токенів
            var token = GenerateSimpleToken(user.Id, user.Email);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            // Отримання ролей користувача
            var roles = await GetUserRolesAsync(user.Id);

            return new AuthResultDto(
                token,
                refreshToken,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                roles
            );
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Невірний email або пароль");
            }

            // Генерація токенів
            var token = GenerateSimpleToken(user.Id, user.Email);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            // Отримання ролей користувача
            var roles = await GetUserRolesAsync(user.Id);

            return new AuthResultDto(
                token,
                refreshToken,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                roles
            );
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiresOn > DateTime.UtcNow);

            if (token == null || token.User == null)
            {
                throw new UnauthorizedAccessException("Невірний або прострочений refresh token");
            }

            // Генерація нового токена
            var newToken = GenerateSimpleToken(token.User.Id, token.User.Email);
            var newRefreshToken = await GenerateRefreshTokenAsync(token.User.Id);

            // Видалення старого refresh token
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();

            // Отримання ролей користувача
            var roles = await GetUserRolesAsync(token.User.Id);

            return new AuthResultDto(
                newToken,
                newRefreshToken,
                token.User.Id,
                token.User.Email,
                token.User.FirstName,
                token.User.LastName,
                roles
            );
        }

        public async Task<bool> AssignRoleAsync(int userId, string roleName)
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

            // Перевірка чи роль вже призначена
            var existingUserRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

            if (existingUserRole != null)
            {
                return true; // Роль вже призначена
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.Id
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role!.Name)
                .ToListAsync();

            return userRoles;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        private string GenerateSimpleToken(int userId, string email)
        {
            // Для MVP використовуємо простий токен
            // У продакшені треба використовувати JWT
            var payload = $"{userId}:{email}:{DateTime.UtcNow:yyyyMMddHHmmss}";
            var bytes = Encoding.UTF8.GetBytes(payload);
            return Convert.ToBase64String(bytes);
        }

        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var token = Guid.NewGuid().ToString();
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresOn = DateTime.UtcNow.AddDays(30),
                CreatedOn = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return token;
        }
    }
}

