namespace MES.Shared;

public class OrderDTO
{
    public long OrderID { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public string Priority { get; set; }
    public DateTime Deadline { get; set; }
    public string Status { get; set; }
}
