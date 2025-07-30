using MES.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using MES.Shared;

namespace MES_API.Controllers;

[ApiController]
[Route("api/production-plans")]
public class ProductionPlanController : ControllerBase
{
    private readonly IProductionPlanService _planService;

    public ProductionPlanController(IProductionPlanService planService)
    {
        _planService = planService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var plans = _planService.GetAll();
        return Ok(plans);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var plan = _planService.GetById(id);
        return plan != null ? Ok(plan) : NotFound();
    }

    [HttpPost]
    public IActionResult Create([FromBody] ProductionPlanDTO dto)
    {
        var id = _planService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ProductionPlanDTO dto)
    {
        _planService.Update(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _planService.Delete(id);
        return NoContent();
    }
}
