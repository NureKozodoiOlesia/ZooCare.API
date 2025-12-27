using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public AnimalService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<AnimalDto>> GetAllAnimalsAsync()
        {
            var animals = await _context.Animals
                .Include(a => a.Enclosure)
                .ToListAsync();

            return animals.Select(a => new AnimalDto(
                a.Id,
                a.EnclosureId,
                a.Enclosure?.Name ?? "Unknown",
                a.Name,
                a.Species,
                a.Age
            ));
        }

        public async Task<AnimalDto?> GetAnimalByIdAsync(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                return null;
            }

            return new AnimalDto(
                animal.Id,
                animal.EnclosureId,
                animal.Enclosure?.Name ?? "Unknown",
                animal.Name,
                animal.Species,
                animal.Age
            );
        }

        public async Task<IEnumerable<AnimalDto>> GetAnimalsByEnclosureAsync(int enclosureId)
        {
            var animals = await _context.Animals
                .Include(a => a.Enclosure)
                .Where(a => a.EnclosureId == enclosureId)
                .ToListAsync();

            return animals.Select(a => new AnimalDto(
                a.Id,
                a.EnclosureId,
                a.Enclosure?.Name ?? "Unknown",
                a.Name,
                a.Species,
                a.Age
            ));
        }

        public async Task<AnimalDto> CreateAnimalAsync(CreateAnimalDto dto)
        {
            // Перевірка чи вольєр існує
            var enclosure = await _unitOfWork.Enclosures.GetByIdAsync(dto.EnclosureId);
            if (enclosure == null)
            {
                throw new KeyNotFoundException($"Вольєр з ID {dto.EnclosureId} не знайдений");
            }

            var animal = new Animal
            {
                EnclosureId = dto.EnclosureId,
                Name = dto.Name,
                Species = dto.Species,
                Age = dto.Age
            };

            await _unitOfWork.Animals.AddAsync(animal);
            await _unitOfWork.SaveAsync();

            return new AnimalDto(
                animal.Id,
                animal.EnclosureId,
                enclosure.Name,
                animal.Name,
                animal.Species,
                animal.Age
            );
        }

        public async Task<AnimalDto> UpdateAnimalAsync(int id, UpdateAnimalDto dto)
        {
            var animal = await _context.Animals
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
            {
                throw new KeyNotFoundException($"Тварина з ID {id} не знайдена");
            }

            if (dto.EnclosureId.HasValue)
            {
                var enclosure = await _unitOfWork.Enclosures.GetByIdAsync(dto.EnclosureId.Value);
                if (enclosure == null)
                {
                    throw new KeyNotFoundException($"Вольєр з ID {dto.EnclosureId.Value} не знайдений");
                }
                animal.EnclosureId = dto.EnclosureId.Value;
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                animal.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.Species))
            {
                animal.Species = dto.Species;
            }

            if (dto.Age.HasValue)
            {
                animal.Age = dto.Age.Value;
            }

            _unitOfWork.Animals.Update(animal);
            await _unitOfWork.SaveAsync();

            // Отримуємо оновлені дані
            return (await GetAnimalByIdAsync(id))!;
        }

        public async Task<bool> DeleteAnimalAsync(int id)
        {
            var animal = await _unitOfWork.Animals.GetByIdAsync(id);
            if (animal == null)
            {
                return false;
            }

            _unitOfWork.Animals.Delete(animal);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}

