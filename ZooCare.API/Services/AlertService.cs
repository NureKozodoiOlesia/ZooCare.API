using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public AlertService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<AlertDto>> GetAllAlertsAsync()
        {
            var alerts = await _context.SystemAlerts
                .Include(a => a.Enclosure)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return alerts.Select(a => new AlertDto(
                a.Id,
                a.EnclosureId,
                a.Enclosure?.Name ?? "Unknown",
                a.Message,
                a.Severity,
                a.IsResolved,
                a.CreatedAt
            ));
        }

        public async Task<IEnumerable<AlertDto>> GetUnresolvedAlertsAsync()
        {
            var alerts = await _context.SystemAlerts
                .Include(a => a.Enclosure)
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return alerts.Select(a => new AlertDto(
                a.Id,
                a.EnclosureId,
                a.Enclosure?.Name ?? "Unknown",
                a.Message,
                a.Severity,
                a.IsResolved,
                a.CreatedAt
            ));
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsByEnclosureAsync(int enclosureId)
        {
            var alerts = await _context.SystemAlerts
                .Include(a => a.Enclosure)
                .Where(a => a.EnclosureId == enclosureId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return alerts.Select(a => new AlertDto(
                a.Id,
                a.EnclosureId,
                a.Enclosure?.Name ?? "Unknown",
                a.Message,
                a.Severity,
                a.IsResolved,
                a.CreatedAt
            ));
        }

        public async Task<AlertDto?> GetAlertByIdAsync(int id)
        {
            var alert = await _context.SystemAlerts
                .Include(a => a.Enclosure)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alert == null)
            {
                return null;
            }

            return new AlertDto(
                alert.Id,
                alert.EnclosureId,
                alert.Enclosure?.Name ?? "Unknown",
                alert.Message,
                alert.Severity,
                alert.IsResolved,
                alert.CreatedAt
            );
        }

        public async Task<AlertDto> CreateAlertAsync(CreateAlertDto dto)
        {
            // Перевірка чи вольєр існує
            var enclosure = await _unitOfWork.Enclosures.GetByIdAsync(dto.EnclosureId);
            if (enclosure == null)
            {
                throw new KeyNotFoundException($"Вольєр з ID {dto.EnclosureId} не знайдений");
            }

            var alert = new SystemAlert
            {
                EnclosureId = dto.EnclosureId,
                Message = dto.Message,
                Severity = dto.Severity,
                IsResolved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SystemAlerts.AddAsync(alert);
            await _unitOfWork.SaveAsync();

            return (await GetAlertByIdAsync(alert.Id))!;
        }

        public async Task<AlertDto> ResolveAlertAsync(int id)
        {
            var alert = await _unitOfWork.SystemAlerts.GetByIdAsync(id);
            if (alert == null)
            {
                throw new KeyNotFoundException($"Сповіщення з ID {id} не знайдене");
            }

            alert.IsResolved = true;

            _unitOfWork.SystemAlerts.Update(alert);
            await _unitOfWork.SaveAsync();

            return (await GetAlertByIdAsync(id))!;
        }

        public async Task<bool> DeleteAlertAsync(int id)
        {
            var alert = await _unitOfWork.SystemAlerts.GetByIdAsync(id);
            if (alert == null)
            {
                return false;
            }

            _unitOfWork.SystemAlerts.Delete(alert);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}

