

using SharedModels;

namespace MES.API.Controllers;

public interface IProductionPlanService
{
    IEnumerable<ProductionPlanDto> GetAll();
    ProductionPlanDto GetById(int id);
    long Create(ProductionPlanDto dto);
    void Update(int id, ProductionPlanDto dto);
    void Delete(int id);
}