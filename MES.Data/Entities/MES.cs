// Сущности БД
namespace MES.Data.Models
{
   
    // Order.cs
    public class Order
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    // Equipment.cs
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
