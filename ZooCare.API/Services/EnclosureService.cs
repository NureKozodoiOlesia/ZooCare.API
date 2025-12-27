using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class EnclosureService : IEnclosureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public EnclosureService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<EnclosureDto>> GetAllEnclosuresAsync()
        {
            var enclosures = await _context.Enclosures
                .Include(e => e.Animals)
                .ToListAsync();

            return enclosures.Select(e => new EnclosureDto(
                e.Id,
                e.Name,
                e.Type,
                e.Location,
                e.Animals.Count
            ));
        }

        public async Task<EnclosureDto?> GetEnclosureByIdAsync(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
            {
                return null;
            }

            return new EnclosureDto(
                enclosure.Id,
                enclosure.Name,
                enclosure.Type,
                enclosure.Location,
                enclosure.Animals.Count
            );
        }

        public async Task<EnclosureDto> CreateEnclosureAsync(CreateEnclosureDto dto)
        {
            var enclosure = new Enclosure
            {
                Name = dto.Name,
                Type = dto.Type,
                Location = dto.Location
            };

            await _unitOfWork.Enclosures.AddAsync(enclosure);
            await _unitOfWork.SaveAsync();

            return new EnclosureDto(
                enclosure.Id,
                enclosure.Name,
                enclosure.Type,
                enclosure.Location,
                0
            );
        }

        public async Task<EnclosureDto> UpdateEnclosureAsync(int id, UpdateEnclosureDto dto)
        {
            var enclosure = await _unitOfWork.Enclosures.GetByIdAsync(id);
            if (enclosure == null)
            {
                throw new KeyNotFoundException($"Вольєр з ID {id} не знайдений");
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                enclosure.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.Type))
            {
                enclosure.Type = dto.Type;
            }

            if (!string.IsNullOrEmpty(dto.Location))
            {
                enclosure.Location = dto.Location;
            }

            _unitOfWork.Enclosures.Update(enclosure);
            await _unitOfWork.SaveAsync();

            // Отримуємо оновлені дані
            return (await GetEnclosureByIdAsync(id))!;
        }

        public async Task<bool> DeleteEnclosureAsync(int id)
        {
            var enclosure = await _context.Enclosures
                .Include(e => e.Animals)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enclosure == null)
            {
                return false;
            }

            // Перевірка чи є тварини у вольєрі
            if (enclosure.Animals.Any())
            {
                throw new InvalidOperationException("Неможливо видалити вольєр, в якому є тварини");
            }

            _unitOfWork.Enclosures.Delete(enclosure);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<AnimalDto>> GetAnimalsInEnclosureAsync(int enclosureId)
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
    }
}

