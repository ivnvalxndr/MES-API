namespace MES.Shared
{
    public class ProductionPlanDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
