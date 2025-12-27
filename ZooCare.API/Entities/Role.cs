using System.ComponentModel.DataAnnotations;

namespace ZooCare.API.Entities
{
    public class Role
    {
        public int Id { get; set; }
        [MaxLength(256)] public string Name { get; set; } = string.Empty;
        [MaxLength(256)] public string NormalizedName { get; set; } = string.Empty;
        public List<UserRole> UserRoles { get; set; } = new();
    }
}