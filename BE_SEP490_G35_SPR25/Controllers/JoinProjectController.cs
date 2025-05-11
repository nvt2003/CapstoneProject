using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs.JoinedProjects;
using Planify_BackEnd.Services.Events;
using Planify_BackEnd.Services.JoinProjects;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoinProjectController : ControllerBase
    {
        private readonly IJoinProjectService _joinProjectService;
        public JoinProjectController(IJoinProjectService joinProjectService)
        {
            _joinProjectService = joinProjectService;
        }
        [HttpGet("get-list-event-joined")]
        [Authorize(Roles = "Implementer")]
        public async Task<IActionResult> GetJoiningEvenets(int page, int pageSize)
        {
            try
            {
                var userId =Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ;
                var response =  _joinProjectService.JoiningEvents( page, pageSize, userId);
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any task");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("view-attended-events-history")]
        [Authorize(Roles = "Implementer")]
        public async Task<IActionResult> GetAttendedEventsHistory(int page, int pageSize)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var response =  _joinProjectService.AttendedEvents( page, pageSize, userId);
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any task");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("delete-from-project/{eventId}/{userId}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> DeleteImplementerFromEvent(int eventId, Guid userId)
        {
            try
            {
                if (await _joinProjectService.DeleteImplementerFromEvent(userId, eventId))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Cannot remove implementer from event!");
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-implementers")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> AddImplementersToEvent([FromBody] AddImplementersToEventDTO request)
        {
            var response = await _joinProjectService.AddImplementersToEventAsync(request);
            return StatusCode(response.Status, response);
        }

        [HttpGet("search-implement-joined-project")]
        [Authorize(Roles = "Event Organizer, Implementer, Campus Manager")]
        public async Task<IActionResult> SearchImplementJoinedProjects(int page, int pageSize, int? eventId,
            string? email,
            string? name)
        {
            try
            {
                var response = await _joinProjectService.SearchImplementerJoinedEvent(page, pageSize,eventId,email,name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
