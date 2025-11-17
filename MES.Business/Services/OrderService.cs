using MES.Business.Interfaces;
using MES.Data.Entities;
using MES.Data.Enums;
using MES.Data.Interfaces;
using MES.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace MES.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllAsync(int userId, UserRole userRole)
        {
            _logger.LogInformation("User {UserId} with role {UserRole} is accessing orders", userId, userRole);

            IEnumerable<Order> orders = userRole == UserRole.Operator
                ? await _orderRepository.GetOrdersByOperatorAsync(userId)
                : await _orderRepository.GetOrdersWithDetailsAsync();

            return orders.Select(MapToDTO);
        }

        public async Task<OrderDTO?> GetByIdAsync(long orderId, int userId, UserRole userRole)
        {
            var canAccess = await _orderRepository.CanUserAccessOrderAsync(orderId, userId, userRole);
            if (!canAccess)
                throw new UnauthorizedAccessException("Доступ к заказу запрещен");

            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            return order != null ? MapToDTO(order) : null;
        }

        public async Task<long> CreateAsync(OrderDTO dto, int userId)
        {
            var order = new Order
            {
                ProductionPlanID = dto.ProductionPlanID,
                Quantity = dto.Quantity,
                Priority = dto.Priority,
                Deadline = dto.Deadline,
                Status = dto.Status,
                AssignedOperatorId = dto.AssignedOperatorId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            var createdOrder = await _orderRepository.AddAsync(order);
            _logger.LogInformation("Order {OrderId} created by user {UserId}", createdOrder.OrderID, userId);

            return createdOrder.OrderID;
        }

        public async Task UpdateAsync(long orderId, OrderDTO dto, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found");

            // Проверка прав доступа
            if (!await CanUserModifyOrderAsync(userId))
                throw new UnauthorizedAccessException("Недостаточно прав для редактирования заказа");

            order.ProductionPlanID = dto.ProductionPlanID;
            order.Quantity = dto.Quantity;
            order.Priority = dto.Priority;
            order.Deadline = dto.Deadline;
            order.AssignedOperatorId = dto.AssignedOperatorId;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order {OrderId} updated by user {UserId}", orderId, userId);
        }

        public async Task DeleteAsync(long orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found");

            // Только админы могут удалять
            if (!await IsUserAdminAsync(userId))
                throw new UnauthorizedAccessException("Недостаточно прав для удаления заказа");

            await _orderRepository.DeleteAsync(order);
            _logger.LogInformation("Order {OrderId} deleted by user {UserId}", orderId, userId);
        }

        public async Task<bool> StartOrderAsync(long orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            if (order.AssignedOperatorId != userId)
                throw new UnauthorizedAccessException("Только назначенный оператор может запускать заказ");

            // Простая проверка enum - без преобразования!
            if (order.Status != EntityStatus.Draft && order.Status != EntityStatus.Planned)
                throw new InvalidOperationException("Заказ можно запустить только из статуса 'Draft' или 'Planned'");

            order.Status    = EntityStatus.InProgress;
            order.StartedAt = DateTime.UtcNow;
            order.StartedBy = userId;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> CompleteOrderAsync(long orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            if (order.Status != EntityStatus.InProgress)
                throw new InvalidOperationException("Заказ можно завершить только из статуса 'InProgress'");

            order.Status = EntityStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
            order.CompletedBy = userId;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order {OrderId} completed by user {UserId}", orderId, userId);

            return true;
        }

        public async Task<bool> CancelOrderAsync(long orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = EntityStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order {OrderId} cancelled by user {UserId}", orderId, userId);

            return true;
        }

        public async Task<bool> UpdateStatusAsync(long orderId, EntityStatus status, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order {OrderId} status updated to {Status} by user {UserId}", orderId, status, userId);

            return true;
        }

        public async Task<bool> AssignOperatorAsync(long orderId, int operatorId, int managerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            // Проверяем, что назначающий - менеджер или админ
            if (!await CanUserManageOrdersAsync(managerId))
                throw new UnauthorizedAccessException("Только менеджеры и администраторы могут назначать операторов");

            order.AssignedOperatorId = operatorId;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Order {OrderId} assigned to operator {OperatorId} by manager {ManagerId}",
                orderId, operatorId, managerId);

            return true;
        }

        public async Task<IEnumerable<OrderDTO>> GetMyOrdersAsync(int operatorId)
        {
            var orders = await _orderRepository.GetOrdersByOperatorAsync(operatorId);
            return orders.Select(MapToDTO);
        }

        // Вспомогательные методы
        private async Task<bool> CanUserModifyOrderAsync(int userId)
        {
            var userRole = await GetUserRoleAsync(userId);
            return userRole == UserRole.Technologist ||
                   userRole == UserRole.Manager ||
                   userRole == UserRole.Admin;
        }

        private async Task<bool> CanUserManageOrdersAsync(int userId)
        {
            var userRole = await GetUserRoleAsync(userId);
            return userRole == UserRole.Manager || userRole == UserRole.Admin;
        }

        private async Task<bool> IsUserAdminAsync(int userId)
        {
            var userRole = await GetUserRoleAsync(userId);
            return userRole == UserRole.Admin;
        }

        private OrderDTO MapToDTO(Order order)
        {
            return new OrderDTO
            {
                OrderID = order.OrderID,
                ProductionPlanID = order.ProductionPlanID,
                Quantity = order.Quantity,
                Priority = order.Priority,
                Deadline = order.Deadline,
                Status = order.Status,
                AssignedOperatorId = order.AssignedOperatorId,
                AssignedOperatorName = order.AssignedOperator?.UserName ?? string.Empty,
                CreatedBy = order.CreatedBy,
                CreatedByName = order.CreatedByUser?.UserName ?? string.Empty,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                StartedBy = order.StartedBy,
                StartedAt = order.StartedAt,
                CompletedBy = order.CompletedBy,
                CompletedAt = order.CompletedAt,
                IsOverdue = order.IsOverdue
            };
        }

        private async Task<UserRole> GetUserRoleAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Role ?? UserRole.Operator;
        }
    }
}