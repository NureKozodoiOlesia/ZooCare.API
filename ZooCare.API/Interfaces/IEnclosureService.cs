using ZooCare.API.DTOs;
using ZooCare.API.Entities;

namespace ZooCare.API.Interfaces
{
    public interface IEnclosureService
    {
        Task<IEnumerable<EnclosureDto>> GetAllEnclosuresAsync();
        Task<EnclosureDto?> GetEnclosureByIdAsync(int id);
        Task<EnclosureDto> CreateEnclosureAsync(CreateEnclosureDto dto);
        Task<EnclosureDto> UpdateEnclosureAsync(int id, UpdateEnclosureDto dto);
        Task<bool> DeleteEnclosureAsync(int id);
        Task<IEnumerable<AnimalDto>> GetAnimalsInEnclosureAsync(int enclosureId);
    }
}

