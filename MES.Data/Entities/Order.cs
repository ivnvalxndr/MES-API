using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MES.Data.Models;

public class Order : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long OrderID { get; set; }

    // Связь с планом (ProductionPlanID)
    public int ProductionPlanID { get; set; }

    [ForeignKey(nameof(ProductionPlanID))]
    public ProductionPlan Plan { get; set; }

    // Связь с материалом/продуктом (Material)
    /*public int ProductID { get; set; }

    [ForeignKey(nameof(ProductID))]
    public Material Material { get; set; }*/

    public int Quantity { get; set; }

    [Required]
    [StringLength(50)]
    public required string Priority { get; set; }

    public DateTime Deadline { get; set; }

    [Required]
    [StringLength(50)]
    public required string Status { get; set; }
}