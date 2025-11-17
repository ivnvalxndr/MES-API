namespace MES.Shared;

public class AuthResultDTO
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserDTO? User { get; set; }
    public string? ErrorMessage { get; set; }
}