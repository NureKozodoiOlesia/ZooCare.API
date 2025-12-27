using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } // Навігаційна властивість
        [MaxLength(256)] public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}