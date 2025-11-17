using System.ComponentModel.DataAnnotations;
using MES.Data.Enums;

namespace MES.Data.Entities;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Operator;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Order> CreatedOrders { get; set; } = new List<Order>();
    public ICollection<Order> AssignedOrders { get; set; } = new List<Order>();
}