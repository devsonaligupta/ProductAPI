using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result == null)
                return BadRequest(new { message = "Username already exists. Please choose another." });

            return Ok(result);
        }

        
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AuthRequestDto request)
        {
            var result = await _authService.RegisterAsync(request, role: "Admin");

            if (result == null)
                return BadRequest(new { message = "Username already exists. Please choose another." });

            return Ok(result);
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(result);
        }
    }
}
