using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MES.Shared.DTOs;
using MES.Data.Interfaces;
using Microsoft.Extensions.Logging;
using MES.Business.Interfaces;
using MES.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MES.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration configuration,
            IUserRepository userRepository,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AuthResultDTO> AuthenticateAsync(LoginDTO loginDTO)
        {
            _logger.LogInformation("Attempting authentication for user: {Username}", loginDTO.Username);

            var user = await _userRepository.GetByUsernameAsync(loginDTO.Username);

            if (user == null || !VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                _logger.LogWarning("Authentication failed for user: {Username}", loginDTO.Username);
                return new AuthResultDTO { Success = false, ErrorMessage = "Неверное имя пользователя или пароль" };
            }

            await _userRepository.UpdateLastLoginAsync(user.Id);

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
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
                new Claim(ClaimTypes.Name, user.Username),
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
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                LastLogin = user.LastLogin
            } : null;
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // ВРЕМЕННО - замените на BCrypt!
            return password == passwordHash;
        }
    }
}