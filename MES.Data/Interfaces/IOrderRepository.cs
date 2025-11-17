
using MES.Data.Entities;
using MES.Data.Enums;

namespace MES.Data.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByOperatorAsync(int operatorId);
    Task<IEnumerable<Order>> GetOrdersWithDetailsAsync();
    Task<Order?> GetOrderWithDetailsAsync(long orderId);
    Task UpdateStatusAsync(long orderId, OrderStatus status);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<bool> IsOrderAssignedToOperatorAsync(long orderId, int operatorId);
    Task<bool> CanUserAccessOrderAsync(long orderId, int userId, UserRole userRole);
}