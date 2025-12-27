using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    // 2. Zoo Infrastructure
    public class Enclosure
    {
        public int Id { get; set; }
        [MaxLength(128)] public string Name { get; set; } = string.Empty;
        [MaxLength(64)] public string Type { get; set; } = string.Empty;
        [MaxLength(256)] public string Location { get; set; } = string.Empty;

        // Зв'язки
        public List<Animal> Animals { get; set; } = new();
        public List<IoTDevice> Devices { get; set; } = new();
        public List<DailyTask> Tasks { get; set; } = new();
        public List<SystemAlert> Alerts { get; set; } = new();
    }
}