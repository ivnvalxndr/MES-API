using MES.Data.Entities;
using MES.Data.Enums;
using MES.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Data.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByOperatorAsync(int operatorId)
        {
            return await _dbSet
                .Where(o => o.AssignedOperatorId == operatorId)
                .Include(o => o.Plan)
                .Include(o => o.AssignedOperator)
                .Include(o => o.CreatedByUser)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersWithDetailsAsync()
        {
            return await _dbSet
                .Include(o => o.Plan)
                .Include(o => o.AssignedOperator)
                .Include(o => o.CreatedByUser)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithDetailsAsync(long orderId)
        {
            return await _dbSet
                .Include(o => o.Plan)
                .Include(o => o.AssignedOperator)
                .Include(o => o.CreatedByUser)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task UpdateStatusAsync(long orderId, OrderStatus status)
        {
            var order = await _dbSet.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Where(o => o.Status == status)
                .Include(o => o.Plan)
                .Include(o => o.AssignedOperator)
                .ToListAsync();
        }

        public async Task<bool> IsOrderAssignedToOperatorAsync(long orderId, int operatorId)
        {
            return await _dbSet
                .AnyAsync(o => o.OrderID == orderId && o.AssignedOperatorId == operatorId);
        }

        public async Task<bool> CanUserAccessOrderAsync(long orderId, int userId, UserRole userRole)
        {
            if (userRole == UserRole.Operator)
            {
                return await _dbSet
                    .AnyAsync(o => o.OrderID == orderId && o.AssignedOperatorId == userId);
            }

            return await _dbSet.AnyAsync(o => o.OrderID == orderId);
        }
    }
}