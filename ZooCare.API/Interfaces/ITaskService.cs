using ZooCare.API.DTOs;
using ZooCare.API.Entities;

namespace ZooCare.API.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<IEnumerable<TaskDto>> GetMyTasksAsync(int userId);
        Task<IEnumerable<TaskDto>> GetTasksByEnclosureAsync(int enclosureId);
        Task<TaskDto?> GetTaskByIdAsync(int id);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task<TaskDto> UpdateTaskStatusAsync(int id, UpdateTaskStatusDto dto);
        Task<bool> DeleteTaskAsync(int id);
    }
}

