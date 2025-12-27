using Microsoft.EntityFrameworkCore;
using ZooCare.API.Entities;

namespace ZooCare.API.Data
{
    public class ZooContext : DbContext
    {
        public ZooContext(DbContextOptions<ZooContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Animal> Animals { get; set; }

        public DbSet<IoTDevice> IoTDevices { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }

        public DbSet<DailyTask> DailyTasks { get; set; }
        public DbSet<SystemAlert> SystemAlerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        }
    }
}