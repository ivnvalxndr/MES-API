using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MES.Shared.DTOs;
using MES.Data.Interfaces;
using Microsoft.Extensions.Logging;
using MES.Business.Interfaces;
using MES.Data.Entities;
using MES.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MES.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResultDTO> AuthenticateAsync(LoginDTO loginDTO)
        {
            _logger.LogInformation("Attempting authentication for user: {Username}", loginDTO.Username);

            var user = await _userManager.FindByNameAsync(loginDTO.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                _logger.LogWarning("Authentication failed for user: {Username}", loginDTO.Username);
                return new AuthResultDTO { Success = false, ErrorMessage = "Неверное имя пользователя или пароль" };
            }

            // Обновляем LastLogin
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Username = user.UserName, // Теперь UserName вместо Username
                Role = user.Role,
                LastLogin = user.LastLogin
            };

            var token = GenerateJwtToken(userDTO);

            _logger.LogInformation("Authentication successful for user: {Username}", loginDTO.Username);
            return new AuthResultDTO { Success = true, Token = token, User = userDTO };
        }

        public string GenerateJwtToken(UserDTO user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("LastLogin", user.LastLogin?.ToString("o") ?? DateTime.UtcNow.ToString("o"))
            };

            // Теперь SigningCredentials будет работать
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null ? new UserDTO
            {
                Id = user.Id,
                Username = user.UserName,
                Role = user.Role,
                LastLogin = user.LastLogin
            } : null;
        }

        private async Task<bool> VerifyPassword(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}