using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.Services.Dashboards;
using Planify_BackEnd.Services.Events;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("monthly-stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMonthlyStats(int year)
        {
            var stats = await _dashboardService.GetMonthlyStatsAsync(year);
            return Ok(stats);
        }
        [HttpGet("used-categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsedCategories()
        {
            var result = await _dashboardService.GetUsedCategoriesAsync();
            return Ok(result);
        }
        [HttpGet("latest-events")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLatestEvents(string campusName)
        {
            var result = await _dashboardService.GetLatestEventsAsync(campusName);
            return Ok(result);
        }
        [HttpGet("top-events-by-participants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTopEventsByParticipants()
        {
            var result = await _dashboardService.GetTopEventsByParticipantsAsync();
            return Ok(result);
        }
        [HttpGet("percent-events")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPercentByCampus()
        {
            var result = await _dashboardService.GetPercentEventsByCampus();
            return Ok(result);
        }
    }
}
