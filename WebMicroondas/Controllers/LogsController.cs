using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMicroondas.Domain.Interfaces;

namespace WebMicroondas.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _log;
        public LogsController(ILogService log) => _log = log;

        [HttpGet]
        public IActionResult Get() => Ok(new { success = true, data = _log.GetAll() });

        [HttpDelete]
        public IActionResult Clear() { _log.Clear(); return Ok(new { success = true }); }
    }
}
