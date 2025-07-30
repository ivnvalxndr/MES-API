using MES.Data.Models;

namespace MES.Shared;

public static class OrderDTOTranslator
{
    public static OrderDTO ToDTO(this Order entity)
    {
        return new OrderDTO
        {
            OrderID = entity.OrderID,
            Name = entity.Name,
            Quantity = entity.Quantity,
            Priority = entity.Priority,
            Deadline = entity.Deadline,
            Status = entity.Status
        };
    }

    public static Order ToEntity(this OrderDTO dto)
    {
        return new Order
        {
            OrderID = dto.OrderID,
            Name = dto.Name,
            Quantity = dto.Quantity,
            Priority = dto.Priority,
            Deadline = dto.Deadline,
            Status = dto.Status
        };
    }
}