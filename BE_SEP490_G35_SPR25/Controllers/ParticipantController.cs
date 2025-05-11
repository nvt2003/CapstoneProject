using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Services.Participants;
using System.Security.Claims;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _service;

        public ParticipantController(IParticipantService service)
        {
            _service = service;
        }

        [HttpGet("count/{eventId}")]
        public IActionResult GetParticipantCount(int eventId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = _service.GetParticipantCount(eventId, pageNumber, pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpPost("register")]
        public IActionResult RegisterParticipant([FromBody] RegisterEventDTO registerDto)
        {
            if (registerDto == null)
                return BadRequest(new ResponseDTO(400, "Invalid request body", null));

            var response = _service.RegisterParticipant(registerDto);
            return StatusCode(response.Status, response);
        }

        [HttpGet("registered/{userId}")]
        public IActionResult GetRegisteredEvents(Guid userId)
        {
            var response = _service.GetRegisteredEvents(userId);
            return StatusCode(response.Status, response);
        }

        [HttpPost("unregister")]
        public IActionResult UnregisterParticipant([FromBody] RegisterEventDTO unregisterDto)
        {
            if (unregisterDto == null)
                return BadRequest(new ResponseDTO(400, "Invalid request body", null));

            var response = _service.UnregisterParticipant(unregisterDto);
            return StatusCode(response.Status, response);
        }
        [HttpGet("is-register/{eventId}")]
        public IActionResult IsAlreadyRegistered(int eventId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var response = _service.IsAlreadyRegistered(eventId, userId);
                return StatusCode(response.Status, response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
