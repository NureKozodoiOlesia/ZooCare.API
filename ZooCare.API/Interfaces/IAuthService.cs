using ZooCare.API.DTOs;

namespace ZooCare.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<bool> AssignRoleAsync(int userId, string roleName);
        Task<List<string>> GetUserRolesAsync(int userId);
    }
}

