using MES.Data.Models;

namespace MES.Shared;

public class OrderDTOTranslator
{
    public static OrderDTO ToDTO(this Order entity)
    {
        return new OrderDTO
        {
            Id = entity.Id,
            Name = entity.Name,
            Quantity = entity.Quantity
        };
    }

    public static Order ToEntity(this ProductionPlanDTO dto)
    {
        return new Order
        {
            Id = dto.Id,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }
}