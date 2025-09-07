using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMicroondas.Domain.Interfaces;
using WebMicroondas.Infra.Security;
using WebMicroondas.Models;

namespace WebMicroondas.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly TokenService _tokenSvc;
        private readonly ITokenService _tokenService;
        public AuthController(IConfiguration cfg, TokenService tokenSvc, ITokenService tokenService)
        {
            _cfg = cfg; 
            _tokenSvc = tokenSvc;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<object> Login([FromBody] LoginRequest req)
        {
            var user = _cfg["Auth:Username"];
            var passHash = _cfg["Auth:PasswordHash"];
            if (req.Username == user && PasswordHasher.Sha256(req.Password) == passHash)
            {
                var token = _tokenSvc.CreateToken(req.Username);
                return Ok(new { success = true, data = new LoginResponse(token) });
            }
            return Unauthorized(new { success = false, message = "Credenciais inválidas" });
        }

        [HttpPost("validate")]
        [Authorize]
        public IActionResult Validate() => Ok(new { success = true });
    }
}
