
using MES.Data.Models;

namespace SharedModels;

public static class ProductionPlanMapper
{
    public static ProductionPlanDto ToDto(this ProductionPlan entity)
    {
        return new ProductionPlanDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };
    }

    public static ProductionPlan ToEntity(this ProductionPlanDto dto)
    {
        return new ProductionPlan
        {
            Id = dto.Id,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }
}