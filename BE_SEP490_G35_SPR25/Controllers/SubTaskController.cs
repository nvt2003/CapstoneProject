using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.Services.SubTasks;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTasksController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;
        public SubTasksController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }
        /// <summary>
        /// Get sub-task by ID
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <returns></returns>
        [HttpGet("{subTaskId}")]
        [Authorize(Roles = "Implementer, Event Organizer")]
        public async Task<IActionResult> GetSubTaskById(int subTaskId)
        {
            var response = await _subTaskService.GetSubTaskByIdAsync(subTaskId);
            if (response == null)
            {
                return NotFound(new ResponseDTO(404, "Sub-task not found", null));
            }
            return Ok(response);
        }
        /// <summary>
        /// Get sub-tasks by task ID
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpGet("get-by-task/{taskId}")]
        [Authorize(Roles = "Implementer, Event Organizer")]
        public async Task<IActionResult> GetSubTasksByTaskId(int taskId)
        {
            var response = await _subTaskService.GetSubTasksByTaskIdAsync(taskId);
            return Ok(response);
        }
        /// <summary>
        /// Create sub-task
        /// </summary>
        /// <param name="subTaskDTO"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> CreateSubTask([FromBody] SubTaskCreateRequestDTO subTaskDTO)
        {
            var implementerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _subTaskService.CreateSubTaskAsync(subTaskDTO, implementerId);
            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// update sub-task status
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update-status/{subTaskId}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> UpdateSubTaskStatus(int subTaskId, [FromBody] SubTaskUpdateStatusDTO request)
        {

            var response = await _subTaskService.UpdateSubTaskStatusAsync(subTaskId, request.Status);
            return StatusCode(response.Status, response);
        }

        /// <summary>
        /// Update sub-task
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <param name="subTaskDTO"></param>
        /// <returns></returns>
        [HttpPut("update/{subTaskId}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> UpdateSubTask(int subTaskId, [FromBody] SubTaskUpdateRequestDTO subTaskDTO)
        {
            var response = await _subTaskService.UpdateSubTaskAsync(subTaskId, subTaskDTO);
            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// Delete sub-task
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <returns></returns>
        [HttpDelete("delete/{subTaskId}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> DeleteSubTask(int subTaskId)
        {
            var response = await _subTaskService.DeleteSubTaskAsync(subTaskId);
            return StatusCode(response.Status, response);
        }
        [HttpPut("{subTaskId}/amount")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public IActionResult UpdateActualSubTaskAmount(int subTaskId, [FromBody] decimal amount)
        {
            try
            {
                var response = _subTaskService.UpdateActualSubTaskAmount(subTaskId, amount);
                if (!response)
                {
                    return BadRequest("Cannot update subtask amount!");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("assign-subtask")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> AssignSubtask(Guid userId, int subtaskId)
        {
            try
            {
                var id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var response = await _subTaskService.AssignSubTask(id, userId, subtaskId);
                if (!response)
                {
                    return BadRequest("Cannot assign subtask for user " + userId + "!");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("delete-assigned-user")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> DeleteAssignedUser(Guid userId, int subtaskId)
        {
            try
            {
                var response = await _subTaskService.DeleteAssignedUser(userId, subtaskId);
                if (!response)
                {
                    return BadRequest("Cannot delete assigned user " + userId + "!");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("search/v2")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> SearchTasksByGroupId(Guid implementerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _subTaskService.SearchSubTaskByImplementerId(implementerId, startDate, endDate);
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any sub task");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
