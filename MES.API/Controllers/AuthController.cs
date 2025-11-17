using Microsoft.AspNetCore.Mvc;
using MES.Business.Interfaces;
using System.Security.Claims;
using MES.Shared.DTOs;

namespace MES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Имя пользователя и пароль обязательны" });
            }

            var result = await _authService.AuthenticateAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            _logger.LogInformation("User {Username} successfully logged in", loginDto.Username);
            return Ok(new
            {
                token = result.Token,
                user = result.User,
                expiresIn = TimeSpan.FromMinutes(60).TotalSeconds
            });
        }

        [HttpPost("validate")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            _logger.LogInformation("Token validation for user {Username}", username);

            return Ok(new
            {
                userId,
                username,
                role,
                isValid = true
            });
        }

        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
    }
}