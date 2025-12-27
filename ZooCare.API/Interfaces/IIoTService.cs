using ZooCare.API.DTOs;

namespace ZooCare.API.Interfaces
{
    public interface IIoTService
    {
        Task ProcessTelemetryAsync(SensorDataDto data);
        Task ProcessHeartbeatAsync(string serialNumber);
    }
}