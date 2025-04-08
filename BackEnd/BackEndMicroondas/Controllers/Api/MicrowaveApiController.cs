using Business.Exceptions;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndMicroondas.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MicrowaveApiController : ControllerBase
    {
        private readonly IMicrowaveService _microwaveService;
        public MicrowaveApiController(IMicrowaveService microwaveService)
        {
            _microwaveService = microwaveService;
        }

        [HttpPost("start")]
        public IActionResult Start([FromBody] HeatingRequest request)
        {
            try
            {
                _microwaveService.StartHeating(request.TimeSeconds, request.Power);
                return Ok(new { status = "Aquecimento iniciado" });
            }
            catch (MicrowaveException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("pause")]
        public IActionResult PauseOrCancel()
        {
            _microwaveService.PauseOrCancel();
            return Ok(new { status = "Ação de pausa/cancelamento executada" });
        }

        [HttpPost("resume")]
        public IActionResult Resume()
        {
            _microwaveService.ResumeHeating();
            return Ok(new { status = "Aquecimento retomado" });
        }
    }

    public class HeatingRequest
    {
        public int TimeSeconds { get; set; }
        public int? Power { get; set; }
    }
}
