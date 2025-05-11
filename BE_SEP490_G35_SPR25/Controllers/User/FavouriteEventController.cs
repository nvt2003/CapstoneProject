using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.FavouriteEvents;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.Services.FavouriteEvents;
using static Google.Apis.Requests.BatchRequest;

namespace Planify_BackEnd.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteEventController : ControllerBase
    {
        private readonly IFavouriteEventService _favouriteEventService;
        public FavouriteEventController(IFavouriteEventService favouriteEventService)
        {
            _favouriteEventService = favouriteEventService;
        }
        [HttpGet("get-list")]
        [Authorize]
        public async Task<IActionResult> GetAllFavouriteEvents( int page, int pageSize)
        {
            try
            {
                var spectatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                //var spectatorId = Guid.Parse("F64BA8AC-A0AF-4576-A618-E8502C52FD88");
                var result = _favouriteEventService.GetAllFavouriteEventsAsync(page, pageSize, spectatorId);
                if (result.TotalCount == 0)
                {
                    return NotFound("Cannot found any favourite events");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        [HttpPost("create/{eventId}")]
        [Authorize(Roles = "Spectator, Event Organizer, Implementer, Campus Manager")]
        public async Task<IActionResult> CreateFavouriteEvent(int eventId)
        {
            var spectatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _favouriteEventService.CreateFavouriteEventAsync(eventId, spectatorId);

            return StatusCode(response.Status, response);
        }
        [HttpDelete("delete/{eventId}")]
        [Authorize(Roles = "Spectator, Event Organizer, Implementer, Campus Manager")]
        public async Task<IActionResult> DeleteFavouriteEventAsync(int eventId)
        {
            try
            {
               var spectatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _favouriteEventService.DeleteFavouriteEventAsync(eventId, spectatorId);
                if (result == null)
                {
                    return NotFound("Cannot found any favourite events");
                }
                return StatusCode(result.Status, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        [HttpGet("get-list/v2")]
        [Authorize]
        public async Task<IActionResult> GetFavouriteEventsByUserId(int page, int pageSize)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = User.Claims.FirstOrDefault(c => c.Type == "campusId");
                var result = _favouriteEventService.GetFavouriteEventsByUserId(page, pageSize, userId, int.Parse(campusClaim.Value));
                if (result.TotalCount == 0)
                {
                    return NotFound("Cannot found any favourite events");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
