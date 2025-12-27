using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні завдань", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = $"Завдання з ID {id} не знайдене" });
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні завдання", error = ex.Message });
            }
        }

        [HttpGet("my-tasks/{userId}")]
        public async Task<IActionResult> GetMyTasks(int userId)
        {
            try
            {
                var tasks = await _taskService.GetMyTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні завдань", error = ex.Message });
            }
        }

        [HttpGet("enclosure/{enclosureId}")]
        public async Task<IActionResult> GetTasksByEnclosure(int enclosureId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByEnclosureAsync(enclosureId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні завдань", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                var task = await _taskService.CreateTaskAsync(dto);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при створенні завдання", error = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            try
            {
                var task = await _taskService.UpdateTaskStatusAsync(id, dto);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при оновленні статусу завдання", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Завдання {id} успішно видалене" });
                }
                return NotFound(new { message = $"Завдання з ID {id} не знайдене" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при видаленні завдання", error = ex.Message });
            }
        }
    }
}