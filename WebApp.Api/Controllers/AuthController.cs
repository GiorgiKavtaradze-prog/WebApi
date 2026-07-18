using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.DTOs;
using WebApp.Api.Services.Interfaces;

namespace WebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var authResponse = await _authService.RegisterAsync(dto);
            if (authResponse == null)
            {
                return BadRequest("Registration failed.");
            }
            return Ok(authResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var authResponse = await _authService.LoginAsync(dto);
            if (authResponse == null)
            {
                return Unauthorized("Invalid credentials.");
            }
            return Ok(authResponse);
        }
    }
}
