using System.ComponentModel.DataAnnotations;
using MES.Data.Enums;

namespace MES.Shared.DTOs;

public class OrderDTO
{
    public long OrderID { get; set; }

    [Required]
    public long ProductionPlanID { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public OrderPriority Priority { get; set; } = OrderPriority.Medium;

    [Required]
    public DateTime Deadline { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Draft;

    public int? AssignedOperatorId { get; set; }
    public string? AssignedOperatorName { get; set; }

    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? StartedBy { get; set; }
    public DateTime? StartedAt { get; set; }
    public int? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }

    public bool IsOverdue { get; set; }

    // Вычисляемые свойства для удобства
    public string PriorityName => Priority.ToString();
    public string StatusName => Status.ToString();
}

