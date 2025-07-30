using MES.API.Controllers;
using MES.Data;
using MES.Shared;
using Microsoft.Extensions.Logging;

namespace MES.Business.Services;

public class ProductionPlanService : IProductionPlanService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductionPlanService> _logger;

    public ProductionPlanService(AppDbContext db, ILogger<ProductionPlanService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public IEnumerable<ProductionPlanDTO> GetAll()
    {
        return _db.ProductionPlans
            .AsEnumerable()
            .Select(p => p.ToDTO());
    }

    public ProductionPlanDTO GetById(int id)
    {
        var entity = _db.ProductionPlans.Find(id);
        return entity != null ? ProductionPlanTranslator.ToDTO(entity) : null;
    }

    public long Create(ProductionPlanDTO dto)
    {
        var entity = dto.ToEntity();
        _db.ProductionPlans.Add(entity);
        _db.SaveChanges();
        _logger.LogInformation("Created production plan with ID {PlanId}", entity.ProductionPlanID);
        return entity.ProductionPlanID;
    }

    public void Update(int id, ProductionPlanDTO dto)
    {
        var entity = _db.ProductionPlans.Find(id);
        if (entity != null)
        {
            entity.Name = dto.Name;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            _db.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var entity = _db.ProductionPlans.Find(id);
        if (entity != null)
        {
            _db.ProductionPlans.Remove(entity);
            _db.SaveChanges();
        }
    }

    

    // В методах:
    

    // Пример метода с обратным маппингом
    /*public void AddPlan(ProductionPlanDtoMapper planDto)
    {
        var entity = planDto.ToEntity();
        _db.ProductionPlans.Add(entity);
        _db.SaveChanges();
    }*/
}