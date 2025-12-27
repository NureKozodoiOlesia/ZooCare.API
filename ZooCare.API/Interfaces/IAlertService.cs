using ZooCare.API.DTOs;
using ZooCare.API.Entities;

namespace ZooCare.API.Interfaces
{
    public interface IAlertService
    {
        Task<IEnumerable<AlertDto>> GetAllAlertsAsync();
        Task<IEnumerable<AlertDto>> GetUnresolvedAlertsAsync();
        Task<IEnumerable<AlertDto>> GetAlertsByEnclosureAsync(int enclosureId);
        Task<AlertDto?> GetAlertByIdAsync(int id);
        Task<AlertDto> CreateAlertAsync(CreateAlertDto dto);
        Task<AlertDto> ResolveAlertAsync(int id);
        Task<bool> DeleteAlertAsync(int id);
    }
}

