using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IoTController : ControllerBase
    {
        private readonly IIoTService _iotService;

        public IoTController(IIoTService iotService)
        {
            _iotService = iotService;
        }

        [HttpPost("telemetry")]
        public async Task<IActionResult> ReceiveData([FromBody] SensorDataDto data)
        {
            try
            {
                await _iotService.ProcessTelemetryAsync(data);
                return Ok(new { message = "Дані успішно оброблено", processed = true });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при обробці даних", error = ex.Message });
            }
        }

        [HttpPost("heartbeat")]
        public IActionResult Heartbeat([FromQuery] string serialNumber)
        {
            // Heartbeat логіка може бути додана пізніше
            return Ok(new { message = "Heartbeat received", timestamp = DateTime.UtcNow });
        }
    }
}