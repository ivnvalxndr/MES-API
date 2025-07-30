namespace MES.Data.Models;

public class ProductionPlan : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}