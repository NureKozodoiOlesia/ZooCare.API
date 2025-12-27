using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertsController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlerts()
        {
            try
            {
                var alerts = await _alertService.GetAllAlertsAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні сповіщень", error = ex.Message });
            }
        }

        [HttpGet("unresolved")]
        public async Task<IActionResult> GetUnresolvedAlerts()
        {
            try
            {
                var alerts = await _alertService.GetUnresolvedAlertsAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні сповіщень", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlertById(int id)
        {
            try
            {
                var alert = await _alertService.GetAlertByIdAsync(id);
                if (alert == null)
                {
                    return NotFound(new { message = $"Сповіщення з ID {id} не знайдене" });
                }
                return Ok(alert);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні сповіщення", error = ex.Message });
            }
        }

        [HttpGet("enclosure/{enclosureId}")]
        public async Task<IActionResult> GetAlertsByEnclosure(int enclosureId)
        {
            try
            {
                var alerts = await _alertService.GetAlertsByEnclosureAsync(enclosureId);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні сповіщень", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto dto)
        {
            try
            {
                var alert = await _alertService.CreateAlertAsync(dto);
                return CreatedAtAction(nameof(GetAlertById), new { id = alert.Id }, alert);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при створенні сповіщення", error = ex.Message });
            }
        }

        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveAlert(int id)
        {
            try
            {
                var alert = await _alertService.ResolveAlertAsync(id);
                return Ok(alert);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при вирішенні сповіщення", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(int id)
        {
            try
            {
                var result = await _alertService.DeleteAlertAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Сповіщення {id} успішно видалене" });
                }
                return NotFound(new { message = $"Сповіщення з ID {id} не знайдене" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при видаленні сповіщення", error = ex.Message });
            }
        }
    }
}

