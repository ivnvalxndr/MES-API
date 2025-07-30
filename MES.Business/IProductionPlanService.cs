﻿using MES.Shared;

namespace MES.API.Controllers;

public interface IProductionPlanService
{
    IEnumerable<ProductionPlanDTO> GetAll();
    ProductionPlanDTO GetById(int id);
    long Create(ProductionPlanDTO dto);
    void Update(int id, ProductionPlanDTO dto);
    void Delete(int id);
}