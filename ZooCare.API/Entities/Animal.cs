using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    public class Animal
    {
        public int Id { get; set; }
        public int EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }
        [MaxLength(128)] public string Name { get; set; } = string.Empty;
        [MaxLength(128)] public string Species { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}