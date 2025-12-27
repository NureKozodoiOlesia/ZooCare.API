using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class IoTService : IIoTService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;
        private const decimal CRITICAL_WATER_THRESHOLD = 15m;

        public IoTService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task ProcessTelemetryAsync(SensorDataDto data)
        {
            // 1. Знаходимо пристрій за серійним номером
            var device = await _context.IoTDevices
                .Include(d => d.Enclosure)
                .FirstOrDefaultAsync(d => d.SerialNumber == data.SerialNumber);

            if (device == null)
            {
                throw new KeyNotFoundException($"Пристрій з серійним номером {data.SerialNumber} не знайдений");
            }

            // Оновлюємо статус пристрою та час останнього heartbeat
            device.Status = "Online";
            device.LastHeartbeat = DateTime.UtcNow;
            _unitOfWork.IoTDevices.Update(device);

            // 2. Визначаємо одиницю виміру на основі типу сенсора
            var unit = data.SensorType == "WaterLevel" ? "%" : "unknown";

            // 3. Зберігаємо показник (History)
            var reading = new SensorReading
            {
                DeviceId = device.Id,
                SensorType = data.SensorType,
                Value = data.Value,
                Unit = unit,
                RecordedAt = DateTime.UtcNow
            };

            await _unitOfWork.SensorReadings.AddAsync(reading);

            // 4. БІЗНЕС-ЛОГІКА: Перевірка на критичний рівень води
            if (data.SensorType == "WaterLevel" && data.Value < CRITICAL_WATER_THRESHOLD)
            {
                // Перевіряємо чи вже є невирішене критичне сповіщення для цього вольєра
                var existingAlert = await _context.SystemAlerts
                    .FirstOrDefaultAsync(a => 
                        a.EnclosureId == device.EnclosureId && 
                        !a.IsResolved && 
                        a.Severity == "Critical" &&
                        a.Message.Contains("Water level"));

                if (existingAlert == null)
                {
                    // Створюємо системне сповіщення (Alert)
                    var alert = new SystemAlert
                    {
                        EnclosureId = device.EnclosureId,
                        Message = $"НИЗЬКИЙ РІВЕНЬ ВОДИ: Вольєр {device.Enclosure?.Name ?? device.EnclosureId.ToString()}. Поточний рівень: {data.Value}%",
                        Severity = "Critical",
                        IsResolved = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.SystemAlerts.AddAsync(alert);
                }
            }

            // 5. Зберігаємо всі зміни (Транзакція)
            await _unitOfWork.SaveAsync();
        }
    }
}