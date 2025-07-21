using ExpenseTrackerNet.Server.Models;
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
        public async Task<ActionResult<User>> Register(UserDTO request)
        { 
            var user = await _authService.RegisterAsync(request);
            if(user == null)
            {
                return BadRequest("User registration failed.");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            var tokenResponse = await _authService.LoginAsync(request);
            if (tokenResponse == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok("You are authenticated");

            return Unauthorized("❌ You are not authenticated");
        }


    }
}
