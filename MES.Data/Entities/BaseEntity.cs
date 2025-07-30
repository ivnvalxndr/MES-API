namespace MES.Data.Models;

public class BaseEntity
{
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}