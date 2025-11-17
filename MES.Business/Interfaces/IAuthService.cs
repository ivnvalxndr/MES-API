using MES.Shared;
using MES.Shared.DTOs;

namespace MES.Business.Interfaces;

public interface IAuthService
{
    Task<AuthResultDTO> AuthenticateAsync(LoginDTO loginDto);
    string GenerateJwtToken(UserDTO user);
    Task<UserDTO?> GetUserByIdAsync(int userId);
}