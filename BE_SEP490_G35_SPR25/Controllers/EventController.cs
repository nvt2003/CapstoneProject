using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs.Medias;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Categories;
using Planify_BackEnd.Services.Campus;
using Planify_BackEnd.Services.Categories;
using Planify_BackEnd.Services.Events;
using Planify_BackEnd.Services.Medias;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly ICampusService _campusService;
        private readonly IMediumService _mediumService;

        public EventsController(IEventService eventService, ICampusService campusService, ICategoryService categoryService,IMediumService mediumService)
        {
            _eventService = eventService;
            _campusService = campusService;
            _categoryService = categoryService;
            _mediumService = mediumService;
        }

        /// <summary>
        /// Retrieves all events with related data.
        /// </summary>
        /// <returns>A list of all events.</returns>
        [HttpGet ("list")]
        [Authorize(Roles = "Event Organizer, Implementer, Campus Manager")]
        public async Task<IActionResult> GetAllEvents(int page, int pageSize)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "campusId").Value);
                var response =  _eventService.GetAllEvent(campusClaim, page,  pageSize, userId);
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any event");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
        /// <summary>
        /// Create a new event.
        /// </summary>
        /// <param name="eventDTO"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "Event Organizer, Campus Manager")]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateRequestDTO eventDTO)
        {
            var organizerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var response = await _eventService.CreateEventAsync(eventDTO, organizerId);

            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// Upload image for event.
        /// </summary>
        /// <param name="imageDTO"></param>
        /// <returns></returns>
        [HttpPost("upload-image")]
        [Authorize(Roles = "Event Organizer, Campus Manager")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequestDTO imageDTO)
        {
            var response = await _eventService.UploadImageAsync(imageDTO);

            return StatusCode(response.Status, response);
        }
        /// <summary>
        ///     Get event detail by event id.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("get-event-detail")]
        [Authorize(Roles = "Event Organizer, Campus Manager, Implementer")]
        public async Task<IActionResult> GetEventDetail(int eventId)
        {
            var response = await _eventService.GetEventDetailAsync(eventId);

            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// Get event detail for implementer.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("get-event-detail-implementer")]
        [Authorize(Roles = "Implementer")]
        public async Task<IActionResult> GetEventDetailForImp(int eventId)
        {
            var response = await _eventService.GetEventDetailAsync(eventId);

            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// Update event.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> UpdateEvent(int id,[FromBody] EventUpdateDTO eventDTO)
        {
            try
            {
                if (id != eventDTO.Id)
                {
                    return BadRequest("Not allow update id");
                }

                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = User.Claims.FirstOrDefault(c => c.Type == "campusId");
                eventDTO.UpdateBy = userId;
                eventDTO.CampusId = int.Parse(campusClaim.Value);
                var response = await _eventService.UpdateEventAsync(eventDTO);
                
                return StatusCode(response.Status, response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update-event-media")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> UpdateEventMedia([FromForm] UpdateMediaEvent updateDTO)
        {
            try
            {
                
                var response = await _mediumService.UpdateMediaEvent(updateDTO);
                if (response)
                    return Ok(response);
                else
                    return BadRequest("Error while event media!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Delete event.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("delete/{id}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var response = await _eventService.DeleteEventAsync(id);
                if (response)
                    return Ok();
                else
                    return BadRequest("Cannot delete event!");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Search event by title, start time, end time, min budget, max budget, is public, status, category event id, placed.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="minBudget"></param>
        /// <param name="maxBudget"></param>
        /// <param name="isPublic"></param>
        /// <param name="status"></param>
        /// <param name="CategoryEventId"></param>
        /// <param name="placed"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(Roles = "Event Organizer, Implementer, Campus Manager")]
        public async Task<IActionResult> SearchEventAsync(int page, int pageSize, string? title, 
            DateTime? startTime, DateTime? endTime, decimal? minBudget, decimal? maxBudget, 
            int? isPublic, int? status, int? CategoryEventId, string? placed, Guid? createBy)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = User.Claims.FirstOrDefault(c => c.Type == "campusId");
                var response = await _eventService.SearchEventAsync(page, pageSize, title, startTime, endTime,
                minBudget, maxBudget, isPublic, status, CategoryEventId, placed, userId,int.Parse(campusClaim.Value), createBy);
                if (response.TotalCount == 0)
                    return NotFound("Not found any event");
                return Ok(response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("create-save-draft")]
        //[Authorize(Roles = "Event Organizer, Campus Manager")]
        //public async Task<IActionResult> CreateSaveDraft([FromBody] EventCreateRequestDTO eventDTO)
        //{
        //    var organizerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //    var response = await _eventService.CreateSaveDraft(eventDTO, organizerId);

        //    return StatusCode(response.Status, response);
        //}

        //[HttpPut("update-save-draft")]
        //[Authorize(Roles = "Event Organizer, Campus Manager")]
        //public async Task<IActionResult> UpdateSaveDraft([FromBody] EventDTO eventDTO)
        //{
        //    var organizerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //    var response = await _eventService.UpdateSaveDraft(eventDTO);

        //    return StatusCode(response.Status, response);
        //}
        //[HttpGet("get-save-draft")]
        //[Authorize(Roles = "Event Organizer, Campus Manager")]
        //public async Task<IActionResult> GetSaveDraft()
        //{
        //    var organizerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        //    var response = await _eventService.GetSaveDraft(organizerId);

        //    return StatusCode(response.Status, response);
        //}

        [HttpDelete("events/{eventId}/media")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> DeleteImages(int eventId, [FromBody] DeleteImagesRequestDTO request)
        {
            if (request.EventId != eventId)
            {
                return BadRequest(new ResponseDTO(400, "EventId in URL and body must match", null));
            }

            var response = await _eventService.DeleteImagesAsync(request);
            return StatusCode(response.Status, response);
        }
        [HttpGet("notification")]
        [Authorize]
        public async Task<IActionResult> NotificationIncomingEvent()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var isSuccess = await _eventService.EventIncomingNotification(userId,role);
                if (!isSuccess)
                    return NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-event")]
        [Authorize(Roles = "Event Organizer, Campus Manager")]
        public async Task<IActionResult> MyEvent(int page, int pageSize)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var campusClaim = User.Claims.FirstOrDefault(c => c.Type == "campusId");
                var response = await _eventService.MyEvent(page, pageSize,userId, int.Parse(campusClaim.Value));
                if (response.TotalCount == 0)
                    return NotFound("Not found any event");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
