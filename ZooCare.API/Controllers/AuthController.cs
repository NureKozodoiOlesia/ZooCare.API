using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при реєстрації", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при вході", error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при оновленні токена", error = ex.Message });
            }
        }

        [HttpPost("assign-role")]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            try
            {
                var result = await _authService.AssignRoleAsync(dto.UserId, dto.RoleName);
                if (result)
                {
                    return Ok(new { message = "Роль успішно призначено" });
                }
                return BadRequest(new { message = "Не вдалося призначити роль" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при призначенні ролі", error = ex.Message });
            }
        }
    }
}