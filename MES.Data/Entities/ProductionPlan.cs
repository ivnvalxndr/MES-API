using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MES.Data.Enums;

namespace MES.Data.Entities;

public class ProductionPlan : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public EntityStatus Status { get; set; } = EntityStatus.Draft;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}