using ZooCare.API.Data;
using ZooCare.API.Entities;

namespace ZooCare.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ZooContext _context;

        public UnitOfWork(ZooContext context)
        {
            _context = context;

            // Ініціалізація репозиторіїв
            Users = new GenericRepository<User>(_context);
            Roles = new GenericRepository<Role>(_context);
            Enclosures = new GenericRepository<Enclosure>(_context);
            Animals = new GenericRepository<Animal>(_context);
            IoTDevices = new GenericRepository<IoTDevice>(_context);
            SensorReadings = new GenericRepository<SensorReading>(_context);
            DailyTasks = new GenericRepository<DailyTask>(_context);
            SystemAlerts = new GenericRepository<SystemAlert>(_context);
        }

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Role> Roles { get; private set; }
        public IGenericRepository<Enclosure> Enclosures { get; private set; }
        public IGenericRepository<Animal> Animals { get; private set; }
        public IGenericRepository<IoTDevice> IoTDevices { get; private set; }
        public IGenericRepository<SensorReading> SensorReadings { get; private set; }
        public IGenericRepository<DailyTask> DailyTasks { get; private set; }
        public IGenericRepository<SystemAlert> SystemAlerts { get; private set; }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}