using ZooCare.API.DTOs;
using ZooCare.API.Entities;

namespace ZooCare.API.Interfaces
{
    public interface IAnimalService
    {
        Task<IEnumerable<AnimalDto>> GetAllAnimalsAsync();
        Task<AnimalDto?> GetAnimalByIdAsync(int id);
        Task<IEnumerable<AnimalDto>> GetAnimalsByEnclosureAsync(int enclosureId);
        Task<AnimalDto> CreateAnimalAsync(CreateAnimalDto dto);
        Task<AnimalDto> UpdateAnimalAsync(int id, UpdateAnimalDto dto);
        Task<bool> DeleteAnimalAsync(int id);
    }
}

