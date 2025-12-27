using ZooCare.API.Entities;

namespace ZooCare.API.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<Enclosure> Enclosures { get; }
        IGenericRepository<Animal> Animals { get; }
        IGenericRepository<IoTDevice> IoTDevices { get; }
        IGenericRepository<SensorReading> SensorReadings { get; }
        IGenericRepository<DailyTask> DailyTasks { get; }
        IGenericRepository<SystemAlert> SystemAlerts { get; }

        Task<int> SaveAsync();
    }
}