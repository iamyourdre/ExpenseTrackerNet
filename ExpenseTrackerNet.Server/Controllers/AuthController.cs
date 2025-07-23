using ExpenseTrackerNet.Shared.Models;
using ExpenseTrackerNet.Server.Services;
using ExpenseTrackerNetApp.ApiService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerNet.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO request)
        {
            if (request == null)
            {
                return BadRequest("Registration data is required.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { Errors = errors });
            }

            var user = await _authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("User registration failed. Username may already exist or data is invalid.");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO request)
        {
            if (request == null)
            {
                return BadRequest("Login data is required.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { Errors = errors });
            }

            var tokenResponse = await _authService.LoginAsync(request);
            if (tokenResponse == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(tokenResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Refresh token data is required.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { Errors = errors });
            }

            var tokenResponse = await _authService.RefreshTokenAsync(request);
            if (tokenResponse == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok("You are authenticated");

            return Unauthorized("You are not authenticated");
        }
    }
}
