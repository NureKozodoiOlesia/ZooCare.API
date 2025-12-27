using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Адміністратор", "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Управління ролями
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _adminService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні ролей", error = ex.Message });
            }
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                var role = await _adminService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new { message = $"Роль з ID {id} не знайдена" });
                }
                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні ролі", error = ex.Message });
            }
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            try
            {
                var role = await _adminService.CreateRoleAsync(dto);
                return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при створенні ролі", error = ex.Message });
            }
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var result = await _adminService.DeleteRoleAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Роль {id} успішно видалена" });
                }
                return NotFound(new { message = $"Роль з ID {id} не знайдена" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при видаленні ролі", error = ex.Message });
            }
        }

        [HttpPost("roles/initialize")]
        public async Task<IActionResult> InitializeRoles()
        {
            try
            {
                var result = await _adminService.InitializeRolesAsync();
                return Ok(new { message = "Ролі успішно ініціалізовано", success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при ініціалізації ролей", error = ex.Message });
            }
        }

        // Управління користувачами та ролями
        [HttpDelete("users/{userId}/roles")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, [FromQuery] string roleName)
        {
            try
            {
                var result = await _adminService.RemoveRoleFromUserAsync(userId, roleName);
                if (result)
                {
                    return Ok(new { message = $"Роль '{roleName}' успішно видалена з користувача {userId}" });
                }
                return NotFound(new { message = "Роль не знайдена або не призначена користувачу" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при видаленні ролі", error = ex.Message });
            }
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            try
            {
                var roles = await _adminService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні ролей користувача", error = ex.Message });
            }
        }

        [HttpPut("users/{userId}/password")]
        public async Task<IActionResult> UpdateUserPassword(int userId, [FromBody] UpdatePasswordDto dto)
        {
            try
            {
                var result = await _adminService.UpdateUserPasswordAsync(userId, dto.NewPassword);
                if (result)
                {
                    return Ok(new { message = "Пароль успішно оновлено" });
                }
                return NotFound(new { message = $"Користувач з ID {userId} не знайдений" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при оновленні пароля", error = ex.Message });
            }
        }

        // Статистика та моніторинг
        [HttpGet("stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var stats = await _adminService.GetSystemStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні статистики", error = ex.Message });
            }
        }
    }
}

