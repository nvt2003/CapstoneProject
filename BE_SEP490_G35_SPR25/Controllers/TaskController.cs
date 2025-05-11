using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Services.Events;
using Planify_BackEnd.Services.Tasks;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.Services.Groups;
using System.IdentityModel.Tokens.Jwt;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        /// <summary>
        /// Get all tasks by event Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("list/{eventId}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> GetAllTasks(int eventId, int page, int pageSize)
        {
            try
            {
                var response = _taskService.GetAllTasks(eventId,  page, pageSize);
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
        /// <summary>
        /// Search tasks by name,orderby start date and end date
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> SearchTasksAsync(int page, int pageSize, string? name, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _taskService.SearchTaskOrderByStartDateAsync(page, pageSize, name, startDate, endDate);
                if (response == null || response.Count() == 0)
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
        [HttpPut("{taskId}/amount")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public IActionResult UpdateActualTaskAmount(int taskId, [FromBody] decimal amount)
        {
            try
            {
                var response = _taskService.UpdateActualTaskAmount(taskId, amount);
                if (!response)
                {
                    return BadRequest("Cannot update task amount!");
                }
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="taskDTO"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateRequestDTO taskDTO)
        {
            var organizerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _taskService.CreateTaskAsync(taskDTO, organizerId);

            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// Update a task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskDTO"></param>
        /// <returns></returns>
        [HttpPut("update/{taskId}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskUpdateRequestDTO taskDTO)
        {
            var response = await _taskService.UpdateTaskAsync(taskId, taskDTO);
            return StatusCode(response.Status, response);
        }
        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HttpDelete("delete/{taskId}")]
        [Authorize(Roles = "Event Organizer")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var response = await _taskService.DeleteTaskAsync(taskId);
            return StatusCode(response.Status, response);
        }
        [HttpGet("{taskId}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public IActionResult GetTask(int taskId)
        {
            try
            {
                var response = _taskService.GetTaskById(taskId);
                if (response== null || response.Id == null)
                {
                    return NotFound("Cannot found any task with id: "+taskId);
                }
                return Ok(response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{taskId}/status/{status}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> ChangeStatus(int taskId, int status)
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");
                Guid userId = Guid.Parse(userIdClaim.Value);

                var task = _taskService.GetTaskById(taskId);
                if (task == null || task.Id == null || task.Id == 0)
                    return NotFound("Cannot found any task with id: " + taskId);
                //var checkLead = await _groupService.CheckLeadGroup(userId, task.GroupId);
                //if (!checkLead)
                //{
                //    return BadRequest("Only group leader can change status of task");
                //}
                var response = await _taskService.changeStatus(taskId, status);
                if (!response)
                {
                    return BadRequest("Cannot change status");
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
        public async Task<IActionResult> SearchTasksByGroupId(int page, int pageSize, Guid implementerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _taskService.SearchSubTaskByImplementerId(page, pageSize, implementerId, startDate, endDate);
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
        [HttpPut("delete/v2/{taskId}")]
        [Authorize(Roles = "Event Organizer, Implementer")]
        public async Task<IActionResult> DeleteTaskV2(int taskId)
        {
            var response = await _taskService.DeleteTaskV2(taskId);
            return StatusCode(response.Status, response);
        }
    }
}
