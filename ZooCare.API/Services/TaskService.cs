using Microsoft.EntityFrameworkCore;
using ZooCare.API.Data;
using ZooCare.API.DTOs;
using ZooCare.API.Entities;
using ZooCare.API.Interfaces;
using ZooCare.API.Repositories;

namespace ZooCare.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ZooContext _context;

        public TaskService(IUnitOfWork unitOfWork, ZooContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _context.DailyTasks
                .Include(t => t.Enclosure)
                .Include(t => t.AssignedUser)
                .ToListAsync();

            return tasks.Select(t => new TaskDto(
                t.Id,
                t.EnclosureId,
                t.Enclosure?.Name ?? "Unknown",
                t.AssignedUserId,
                $"{t.AssignedUser?.FirstName} {t.AssignedUser?.LastName}".Trim(),
                t.Title,
                t.Description,
                t.Status,
                t.DueDate,
                t.CompletedAt
            ));
        }

        public async Task<IEnumerable<TaskDto>> GetMyTasksAsync(int userId)
        {
            var tasks = await _context.DailyTasks
                .Include(t => t.Enclosure)
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            return tasks.Select(t => new TaskDto(
                t.Id,
                t.EnclosureId,
                t.Enclosure?.Name ?? "Unknown",
                t.AssignedUserId,
                $"{t.AssignedUser?.FirstName} {t.AssignedUser?.LastName}".Trim(),
                t.Title,
                t.Description,
                t.Status,
                t.DueDate,
                t.CompletedAt
            ));
        }

        public async Task<IEnumerable<TaskDto>> GetTasksByEnclosureAsync(int enclosureId)
        {
            var tasks = await _context.DailyTasks
                .Include(t => t.Enclosure)
                .Include(t => t.AssignedUser)
                .Where(t => t.EnclosureId == enclosureId)
                .ToListAsync();

            return tasks.Select(t => new TaskDto(
                t.Id,
                t.EnclosureId,
                t.Enclosure?.Name ?? "Unknown",
                t.AssignedUserId,
                $"{t.AssignedUser?.FirstName} {t.AssignedUser?.LastName}".Trim(),
                t.Title,
                t.Description,
                t.Status,
                t.DueDate,
                t.CompletedAt
            ));
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context.DailyTasks
                .Include(t => t.Enclosure)
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            return new TaskDto(
                task.Id,
                task.EnclosureId,
                task.Enclosure?.Name ?? "Unknown",
                task.AssignedUserId,
                $"{task.AssignedUser?.FirstName} {task.AssignedUser?.LastName}".Trim(),
                task.Title,
                task.Description,
                task.Status,
                task.DueDate,
                task.CompletedAt
            );
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            // Перевірка чи вольєр існує
            var enclosure = await _unitOfWork.Enclosures.GetByIdAsync(dto.EnclosureId);
            if (enclosure == null)
            {
                throw new KeyNotFoundException($"Вольєр з ID {dto.EnclosureId} не знайдений");
            }

            // Перевірка чи користувач існує
            var user = await _unitOfWork.Users.GetByIdAsync(dto.AssignedUserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з ID {dto.AssignedUserId} не знайдений");
            }

            var task = new DailyTask
            {
                EnclosureId = dto.EnclosureId,
                AssignedUserId = dto.AssignedUserId,
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                Status = "Pending",
                DueDate = dto.DueDate,
                CompletedAt = null
            };

            await _unitOfWork.DailyTasks.AddAsync(task);
            await _unitOfWork.SaveAsync();

            return (await GetTaskByIdAsync(task.Id))!;
        }

        public async Task<TaskDto> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto dto)
        {
            var task = await _unitOfWork.DailyTasks.GetByIdAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Завдання з ID {id} не знайдене");
            }

            task.Status = dto.Status;

            // Якщо завдання виконано, встановлюємо час виконання
            if (dto.Status == "Done" && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            // Якщо статус змінився з Done на інший, очищаємо час виконання
            else if (dto.Status != "Done" && task.CompletedAt != null)
            {
                task.CompletedAt = null;
            }

            _unitOfWork.DailyTasks.Update(task);
            await _unitOfWork.SaveAsync();

            return (await GetTaskByIdAsync(id))!;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.DailyTasks.GetByIdAsync(id);
            if (task == null)
            {
                return false;
            }

            _unitOfWork.DailyTasks.Delete(task);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}

