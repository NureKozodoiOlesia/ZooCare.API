using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{

    public class SystemAlert
    {
        public int Id { get; set; }
        public int EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }
        [MaxLength(256)] public string Message { get; set; } = string.Empty;
        [MaxLength(16)] public string Severity { get; set; } = string.Empty;
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}