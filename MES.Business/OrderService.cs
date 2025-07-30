using MES.Business.Services;
using MES.Data;
using Microsoft.Extensions.Logging;
using SharedModels;

namespace MES.Business;

public class OrderService
{
    public OrderService(AppDbContext db, ILogger<ProductionPlanService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public IEnumerable<ProductionPlanDto> GetAll()
    {
        return _db.ProductionPlans
            .AsEnumerable()
            .Select(p => p.ToDto());
    }

    public ProductionPlanDto GetById(int id)
    {
        var entity = _db.ProductionPlans.Find(id);
        return entity != null ? ProductionPlanMapper.ToDto(entity) : null;
    }

    public long Create(ProductionPlanDto dto)
    {
        var entity = dto.ToEntity();
        _db.ProductionPlans.Add(entity);
        _db.SaveChanges();
        _logger.LogInformation("Created production plan with ID {PlanId}", entity.Id);
        return entity.Id;
    }

    public void Update(int id, ProductionPlanDto dto)
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
}