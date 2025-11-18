using System.ComponentModel.DataAnnotations;
using MES.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace MES.Data.Entities;

public class User : IdentityUser<int>
{
    [Required]
    public UserRole Role { get; set; } = UserRole.Operator;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Order> CreatedOrders { get; set; } = new List<Order>();
    public ICollection<Order> AssignedOrders { get; set; } = new List<Order>();
}