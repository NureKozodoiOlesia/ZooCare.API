using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    // 4. Operations
    public class DailyTask
    {
        public int Id { get; set; }
        public int EnclosureId { get; set; }
        public Enclosure? Enclosure { get; set; }
        public int AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }

        [MaxLength(256)] public string Title { get; set; } = string.Empty;
        [MaxLength(512)] public string Description { get; set; } = string.Empty;
        [MaxLength(32)] public string Status { get; set; } = string.Empty; // Pending, Done
        public DateTime DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}