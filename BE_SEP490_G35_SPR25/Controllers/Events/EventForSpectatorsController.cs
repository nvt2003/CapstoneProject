using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.Repositories.Events;
using Planify_BackEnd.Services.Events;
using System.Security.Claims;

namespace Planify_BackEnd.Controllers.Events
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventForSpectatorsController : ControllerBase
    {
        private readonly IEventSpectatorService _service;
        public EventForSpectatorsController(IEventSpectatorService service)
        {
            _service = service;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetEvents(int page, int pageSize)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = User.Claims.FirstOrDefault(c => c.Type == "campusId");
                var response = _service.GetEvents(page, pageSize,userId,int.Parse(campusClaim.Value));
                if (response.TotalCount==0)
                {
                    return NotFound("Cannot found any event");
                }
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetEventById(int id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var response = _service.GetEventById(id,userId);
                if (response== null || response.Id == 0)
                {
                    return NotFound("Cannot fount event with id: "+id);
                }
                return Ok(response); 
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("search")]
        [Authorize]
        public IActionResult SearchEvents(int page, int pageSize, string? name, DateTime? startDate, DateTime? endDate, string? placed, int? categoryId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = User.Claims.FirstOrDefault(c => c.Type == "campusId");
                var response = _service.SearchEvent(page, pageSize, name, startDate, endDate,placed,categoryId,userId,int.Parse(campusClaim.Value));
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any event");
                }
                return Ok(response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
