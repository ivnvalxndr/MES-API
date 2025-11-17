using MES.Data.Enums;

namespace MES.Shared;

public class UserDTO
{
    public int Id { get; set; }
    public string? Username { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string RoleName => Role.ToString();
    public DateTime? LastLogin { get; set; }
}