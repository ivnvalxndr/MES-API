using MES.Data.Models;

namespace MES.Shared;

public static class ProductionPlanTranslator
{
    public static ProductionPlanDTO ToDTO(this ProductionPlan entity)
    {
        return new ProductionPlanDTO
        {
            Id = entity.ProductionPlanID,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };
    }

    public static ProductionPlan ToEntity(this ProductionPlanDTO dto)
    {
        return new ProductionPlan
        {
            ProductionPlanID = dto.Id,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }
}