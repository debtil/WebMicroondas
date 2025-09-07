using Microsoft.AspNetCore.Mvc;

namespace WebMicroondas.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { ok = true, timestamp = DateTime.UtcNow });
    }
}
