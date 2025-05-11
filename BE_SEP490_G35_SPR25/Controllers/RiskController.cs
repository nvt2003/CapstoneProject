using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Risk;

[Route("api/[controller]")]
[ApiController]
public class RiskController : ControllerBase
{
    private readonly IRiskService _riskService;

    public RiskController(IRiskService riskService)
    {
        _riskService = riskService;
    }

    [HttpPost]
    [Authorize(Roles = "Event Organizer")]
    public async Task<IActionResult> CreateRisk([FromBody] RiskCreateDTO riskDto)
    {
        var result = await _riskService.CreateRiskAsync(riskDto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Event Organizer")]
    public async Task<IActionResult> UpdateRisk(int id, [FromBody] RiskUpdateDTO riskDto)
    {
        riskDto.Id = id;
        var result = await _riskService.UpdateRiskAsync(riskDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Event Organizer")]
    public async Task<IActionResult> DeleteRisk(int id)
    {
        var result = await _riskService.DeleteRiskAsync(id);
        return Ok(result);
    }
}