using MES.Shared.DTOs;
using MES.Data.Entities;
using MES.Data.Enums;

namespace MES.Shared.Translators
{
    public class OrderDTOTranslator : ITranslator<Order, OrderDTO>
    {
        public OrderDTO ToDTO(Order entity)
        {
            if (entity == null) return null!;

            return new OrderDTO
            {
                OrderID = entity.OrderID,
                ProductionPlanID = entity.ProductionPlanID,
                //ProductionPlanName = entity.Plan?.Name ?? string.Empty,
                Quantity = entity.Quantity,
                Priority = entity.Priority, 
                Deadline = entity.Deadline,
                Status = entity.Status,     
                AssignedOperatorId = entity.AssignedOperatorId,
                AssignedOperatorName = entity.AssignedOperator?.UserName ?? string.Empty,
                CreatedBy = entity.CreatedBy,
                CreatedByName = entity.CreatedByUser?.UserName ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                StartedBy = entity.StartedBy,
                StartedAt = entity.StartedAt,
                CompletedBy = entity.CompletedBy,
                CompletedAt = entity.CompletedAt,
                IsOverdue = entity.Deadline < DateTime.UtcNow && entity.Status != EntityStatus.Completed
            };
        }

        public Order ToEntity(OrderDTO dto)
        {
            if (dto == null) return null!;

            return new Order
            {
                OrderID = dto.OrderID,
                ProductionPlanID = dto.ProductionPlanID,
                Quantity = dto.Quantity,
                Priority = dto.Priority,
                Deadline = dto.Deadline,
                Status = dto.Status,
                AssignedOperatorId = dto.AssignedOperatorId,
                CreatedBy = dto.CreatedBy,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                StartedBy = dto.StartedBy,
                StartedAt = dto.StartedAt,
                CompletedBy = dto.CompletedBy,
                CompletedAt = dto.CompletedAt
            };
        }

        public IEnumerable<OrderDTO> ToDTOs(IEnumerable<Order> entities)
        {
            return entities.Select(ToDTO);
        }

        public IEnumerable<Order> ToEntities(IEnumerable<OrderDTO> dtos)
        {
            return dtos.Select(ToEntity);
        }

        public Order ToNewEntity(OrderDTO dto, int createdByUserId)
        {
            return new Order
            {
                ProductionPlanID = dto.ProductionPlanID,
                Quantity = dto.Quantity,
                Priority = dto.Priority, 
                Deadline = dto.Deadline,
                Status = dto.Status, 
                AssignedOperatorId = dto.AssignedOperatorId,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateEntity(Order existingEntity, OrderDTO dto)
        {
            existingEntity.ProductionPlanID = dto.ProductionPlanID;
            existingEntity.Quantity = dto.Quantity;
            existingEntity.Priority = dto.Priority; 
            existingEntity.Deadline = dto.Deadline;
            existingEntity.Status = dto.Status; 
            existingEntity.AssignedOperatorId = dto.AssignedOperatorId;
            existingEntity.UpdatedAt = DateTime.UtcNow;
        }
    }
}