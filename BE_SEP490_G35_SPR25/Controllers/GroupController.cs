using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Services.Events;
using Planify_BackEnd.Services.Groups;
using Planify_BackEnd.DTOs.Groups;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        [HttpPost("create")]
        //[Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreateRequestDTO groupDTO)
        {
            var organizerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var response = await _groupService.CreateGroupAsync(groupDTO, organizerId);

            return StatusCode(response.Status, response);
        }
    }
}
