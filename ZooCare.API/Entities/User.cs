using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    // 1. User Block
    public class User
    {
        public int Id { get; set; }
        [MaxLength(256)] public string UserName { get; set; } = string.Empty;
        [MaxLength(256)] public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        [MaxLength(128)] public string FirstName { get; set; } = string.Empty;
        [MaxLength(128)] public string LastName { get; set; } = string.Empty;

        // Зв'язки
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public List<UserRole> UserRoles { get; set; } = new();
        public List<DailyTask> AssignedTasks { get; set; } = new();
    }
}