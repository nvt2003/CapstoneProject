using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.CostBreakdown;

[Route("api/[controller]")]
[ApiController]
public class CostController : ControllerBase
{
    private readonly ICostService _costService;

    public CostController(ICostService costService)
    {
        _costService = costService;
    }

    [HttpPost]
    [Authorize(Roles = "Event Organizer")]
    public async Task<IActionResult> CreateCost([FromBody] CostBreakdownCreateDTO costDto)
    {
        var result = await _costService.CreateCostAsync(costDto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Event Organizer")]
    public async Task<IActionResult> UpdateCost(int id, [FromBody] CostBreakdownUpdateDTO costDto)
    {
        costDto.Id = id;
        var result = await _costService.UpdateCostAsync(costDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Event Organizer")]
    public async Task<IActionResult> DeleteCost(int id)
    {
        var result = await _costService.DeleteCostAsync(id);
        return Ok(result);
    }
}