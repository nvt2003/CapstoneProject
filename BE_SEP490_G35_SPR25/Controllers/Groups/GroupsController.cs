using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Groups;
using Planify_BackEnd.Services.Groups;

namespace Planify_BackEnd.Controllers.Groups
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        [HttpPut("allocate-cost/{groupId}/{cost}")]
        [Authorize(Roles = "Event Organizer")]
        public IActionResult AllocateCostToGroup(int groupId, decimal cost)
        {
            try
            {
                bool response = _groupService.AllocateCostToGroup(groupId, cost);
                if (response)
                    return Ok("");
                else
                    return BadRequest("Cannot allocate cost to group id " + groupId);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("add-lead-group/{groupId}/{implementerId}")]
        [Authorize(Roles = "Event Organizer")]
        public IActionResult AddLeadGroup(int groupId,Guid implementerId)
        {
            try
            {
                if (_groupService.AddLeadGroup(groupId, implementerId))
                    return Ok();
                else
                    return BadRequest("Cannot add lead group!");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("remove-lead-group/{groupId}/{implementerId}")]
        [Authorize(Roles = "Event Organizer")]
        public IActionResult RemoveLeadGroup(int groupId, Guid implementerId)
        {
            try
            {
                if (_groupService.RemoveLeadGroup(groupId, implementerId))
                    return Ok();
                else
                    return BadRequest("Cannot remove lead group!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> UpdateGroup(GroupDTO groupDto)
        {
            try
            {
                var response = await _groupService.UpdateGroupAsync(groupDto);
                if (response == null || response.Id == null|| response.Id == 0)
                {
                    return BadRequest("Cannot update group!");
                }
                return Ok(response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            try
            {
                var response = await _groupService.GetGroupByIdAsync(id);
                if (response == null || response.Id == 0)
                {
                    return NotFound("Cannot found any group with id: "+id);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                var group = await _groupService.GetGroupByIdAsync(id);
                if (group == null || group.Id == 0)
                {
                    return NotFound();
                }
                var response = await _groupService.DeleteGroupAsync(id);
                if (!response)
                    return BadRequest("Cannot delete group!");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
