using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooCare.API.Entities
{
    public class SensorReading
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public IoTDevice? Device { get; set; }
        [MaxLength(32)] public string SensorType { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,2)")] public decimal Value { get; set; }
        [MaxLength(8)] public string Unit { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
    }
}