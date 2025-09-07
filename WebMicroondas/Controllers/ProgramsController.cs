using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Models;

namespace WebMicroondas.Controllers
{
    [ApiController]
    [Route("api/programs")]
    [Authorize]
    public class ProgramsController : ControllerBase
    {
        private readonly IProgramService _service;
        public ProgramsController(IProgramService service) => _service = service;

        [HttpGet]
        public IActionResult Get()
        {
            var data = _service.GetAll()
            .Select(p => new ProgramDto(
                p.Id, 
                p.Name, 
                p.Food, 
                p.TimeSeconds, 
                p.Power, 
                p.HeatingChar, 
                p.Instructions, 
                p.IsPredefined
                ));

            return Ok(new { success = true, data });
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProgramDto dto)
        {
            try
            {
                var created = _service.Create(new MicrowaveProgram
                {
                    Name = dto.Name,
                    Food = dto.Food,
                    TimeSeconds = dto.Time,
                    Power = dto.Power,
                    HeatingChar = dto.HeatingChar,
                    Instructions = dto.Instructions
                });
                var resp = new ProgramDto(created.Id, created.Name, created.Food, created.TimeSeconds, created.Power, created.HeatingChar, created.Instructions, created.IsPredefined);
                return Ok(new { success = true, data = resp });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
