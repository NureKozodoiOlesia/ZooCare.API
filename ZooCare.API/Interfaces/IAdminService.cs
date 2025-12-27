using ZooCare.API.DTOs;
using ZooCare.API.Entities;

namespace ZooCare.API.Interfaces
{
    public interface IAdminService
    {
        // Управління ролями
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto> CreateRoleAsync(CreateRoleDto dto);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> InitializeRolesAsync();

        // Управління користувачами та їх ролями
        Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<bool> UpdateUserPasswordAsync(int userId, string newPassword);

        // Статистика та моніторинг
        Task<AdminStatsDto> GetSystemStatsAsync();
    }
}

