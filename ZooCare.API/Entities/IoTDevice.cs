using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    // 3. IoT Devices
    public class IoTDevice
    {
        public int Id { get; set; }
        public int EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }
        [MaxLength(64)] public string SerialNumber { get; set; } = string.Empty;
        [MaxLength(32)] public string DeviceType { get; set; } = string.Empty; // WaterLevel, FoodWeight
        [MaxLength(32)] public string Status { get; set; } = string.Empty; // Online, Offline
        public DateTime LastHeartbeat { get; set; }

        public List<SensorReading> Readings { get; set; } = new();
    }
}