using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnclosuresController : ControllerBase
    {
        private readonly IEnclosureService _enclosureService;

        public EnclosuresController(IEnclosureService enclosureService)
        {
            _enclosureService = enclosureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var enclosures = await _enclosureService.GetAllEnclosuresAsync();
                return Ok(enclosures);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні вольєрів", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var enclosure = await _enclosureService.GetEnclosureByIdAsync(id);
                if (enclosure == null)
                {
                    return NotFound(new { message = $"Вольєр з ID {id} не знайдений" });
                }
                return Ok(enclosure);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні вольєра", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEnclosureDto dto)
        {
            try
            {
                var enclosure = await _enclosureService.CreateEnclosureAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = enclosure.Id }, enclosure);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при створенні вольєра", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEnclosureDto dto)
        {
            try
            {
                var enclosure = await _enclosureService.UpdateEnclosureAsync(id, dto);
                return Ok(enclosure);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при оновленні вольєра", error = ex.Message });
            }
        }

        [HttpGet("{id}/animals")]
        public async Task<IActionResult> GetAnimalsInEnclosure(int id)
        {
            try
            {
                var animals = await _enclosureService.GetAnimalsInEnclosureAsync(id);
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні тварин", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _enclosureService.DeleteEnclosureAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Вольєр {id} успішно видалений" });
                }
                return NotFound(new { message = $"Вольєр з ID {id} не знайдений" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при видаленні вольєра", error = ex.Message });
            }
        }
    }
}