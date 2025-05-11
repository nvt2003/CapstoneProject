using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.JoinGroups;
using Planify_BackEnd.Services.JoinGroups;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JoinGroupController : ControllerBase
    {
        private readonly IJoinGroupService _joinGroupService;

        public JoinGroupController(IJoinGroupService joinGroupService)
        {
            _joinGroupService = joinGroupService;
        }

        [HttpPost("add-implementer")]
        public async Task<IActionResult> AddImplementerToGroup([FromBody] JoinGroupRequestDTO request)
        {
            var response = await _joinGroupService.AddImplementersToGroup(request);
            return StatusCode(response.Status, response);
        }
    }
}
