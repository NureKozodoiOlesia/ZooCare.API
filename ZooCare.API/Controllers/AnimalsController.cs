using Microsoft.AspNetCore.Mvc;
using ZooCare.API.Attributes;
using ZooCare.API.DTOs;
using ZooCare.API.Interfaces;

namespace ZooCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalService _animalService;

        public AnimalsController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnimals()
        {
            try
            {
                var animals = await _animalService.GetAllAnimalsAsync();
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні тварин", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnimalById(int id)
        {
            try
            {
                var animal = await _animalService.GetAnimalByIdAsync(id);
                if (animal == null)
                {
                    return NotFound(new { message = $"Тварина з ID {id} не знайдена" });
                }
                return Ok(animal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні тварини", error = ex.Message });
            }
        }

        [HttpGet("enclosure/{enclosureId}")]
        public async Task<IActionResult> GetAnimalsByEnclosure(int enclosureId)
        {
            try
            {
                var animals = await _animalService.GetAnimalsByEnclosureAsync(enclosureId);
                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при отриманні тварин", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize("Адміністратор", "Admin", "Доглядач", "Caretaker")]
        public async Task<IActionResult> CreateAnimal([FromBody] CreateAnimalDto dto)
        {
            try
            {
                var animal = await _animalService.CreateAnimalAsync(dto);
                return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, animal);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при створенні тварини", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize("Адміністратор", "Admin", "Доглядач", "Caretaker")]
        public async Task<IActionResult> UpdateAnimal(int id, [FromBody] UpdateAnimalDto dto)
        {
            try
            {
                var animal = await _animalService.UpdateAnimalAsync(id, dto);
                return Ok(animal);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при оновленні тварини", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize("Адміністратор", "Admin")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            try
            {
                var result = await _animalService.DeleteAnimalAsync(id);
                if (result)
                {
                    return Ok(new { message = $"Тварина {id} успішно видалена" });
                }
                return NotFound(new { message = $"Тварина з ID {id} не знайдена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Помилка при видаленні тварини", error = ex.Message });
            }
        }
    }
}