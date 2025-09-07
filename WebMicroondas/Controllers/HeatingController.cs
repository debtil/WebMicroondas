using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Models;

namespace WebMicroondas.Controllers
{
    [ApiController]
    [Route("api/heating")]
    [Authorize]
    public class HeatingController : ControllerBase
    {
        private readonly IMicrowaveService _service;
        private readonly IProgramService _programs;
        public HeatingController(IMicrowaveService service, IProgramService programs)
        { 
            _service = service; 
            _programs = programs; 
        }

        [HttpPost("start")]
        public IActionResult Start([FromBody] StartHeatingRequest req)
        {
            try
            {
                Domain.Entities.MicrowaveProgram program = null;
                if (req.ProgramId is not null)
                    program = _programs.GetById(req.ProgramId.Value);
                else if (!string.IsNullOrWhiteSpace(req.ProgramName))
                    program = Enumerable.FirstOrDefault(_programs.GetAll(), p => p.Name == req.ProgramName);

                var time = req.Time ?? (program is null ? 30 : program.TimeSeconds);
                var power = req.Power ?? (program is null ? 10 : program.Power);

                _service.Start(time, power, req.HeatingChar, program);

                return Ok(new { success = true, data = _service.GetStatus() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("status")]
        public IActionResult Status() => Ok(new { success = true, data = _service.GetStatus() });

        [HttpPost("pause")]
        public IActionResult Pause()
        { 
            _service.Pause(); 

            return Ok(new { success = true, data = _service.GetStatus() }); 
        }

        [HttpPost("resume")]
        public IActionResult Resume()
        { 
            _service.Resume(); 

            return Ok(new { success = true, data = _service.GetStatus() }); 
        }

        [HttpPost("cancel")]
        public IActionResult Cancel()
        { 
            _service.Cancel(); 

            return Ok(new { success = true }); 
        }

        [HttpPost("add-time")]
        public IActionResult AddTime([FromBody] AddTimeRequest req)
        {
            try
            {
                var add = req?.AdditionalTime > 0 ? req.AdditionalTime : 30;

                _service.AddTime(add);

                return Ok(new { success = true, data = _service.GetStatus() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("complete")]
        public IActionResult Complete()
        { 
            _service.Cancel(); 
            return Ok(new { success = true }); 
        }
    }
}
