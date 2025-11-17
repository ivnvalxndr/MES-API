using MES.Data.Enums;
using MES.Shared.DTOs;

namespace MES.Business.Interfaces;

public interface IOrderService
{
    // Основные CRUD операции
    Task<IEnumerable<OrderDTO>> GetAllAsync(int userId, UserRole userRole);
    Task<OrderDTO?> GetByIdAsync(long orderId, int userId, UserRole userRole);
    Task<long> CreateAsync(OrderDTO dto, int userId);
    Task UpdateAsync(long orderId, OrderDTO dto, int userId);
    Task DeleteAsync(long orderId, int userId);

    // Операции со статусами
    Task<bool> StartOrderAsync(long orderId, int userId);
    Task<bool> CompleteOrderAsync(long orderId, int userId);
    Task<bool> CancelOrderAsync(long orderId, int userId);
    Task<bool> UpdateStatusAsync(long orderId, EntityStatus status, int userId);

    // Дополнительные операции
    Task<bool> AssignOperatorAsync(long orderId, int operatorId, int managerId);
    Task<IEnumerable<OrderDTO>> GetMyOrdersAsync(int operatorId);
}