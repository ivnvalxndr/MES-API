namespace MES.Data.Entities;

public class ProductionPlan : BaseEntity
{
    public long ProductionPlanID { get; set; }
    // Внешний ключ и навигационное свойство к Order
    public long OrderID { get; set; }  // FK
    public Order Order { get; set; } = null!; // Навигационное свойство
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
}