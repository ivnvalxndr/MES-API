using MES.Business.Services;
using MES.Data;
using Microsoft.Extensions.Logging;
using MES.Shared;

namespace MES.Business;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductionPlanService> _logger;

    public OrderService(AppDbContext db, ILogger<ProductionPlanService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public IEnumerable<OrderDTO> GetAll()
    {
        return _db.Orders
            .AsEnumerable()
            .Select(p => p.ToDTO());
    }

    public OrderDTO GetById(int id)
    {
        var entity = _db.Orders.Find(id);
        return entity != null ? OrderDTOTranslator.ToDTO(entity) : null;
    }

    public long Create(OrderDTO dto)
    {
        var entity = dto.ToEntity();
        _db.Orders.Add(entity);
        _db.SaveChanges();
        _logger.LogInformation("Created Order with ID {OrderId}", entity.OrderID);
        return entity.OrderID;
    }

    public void Update(int id, OrderDTO dto)
    {
        var entity = _db.Orders.Find(id);
        if (entity != null)
        {
            entity.Name = dto.Name;
            //TODO Доделать
            _db.SaveChanges();
        }
    }

    public void Delete(int id)
    {
        var entity = _db.Orders.Find(id);
        if (entity != null)
        {
            _db.Orders.Remove(entity);
            _db.SaveChanges();
        }
    }
}