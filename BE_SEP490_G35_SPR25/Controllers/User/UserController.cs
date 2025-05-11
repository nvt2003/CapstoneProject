using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Services.Users;
using System.Security.Claims;
using static Google.Apis.Requests.BatchRequest;

namespace Planify_BackEnd.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserservice _userService;
        public UsersController(IUserservice userService)
        {
            _userService = userService;
        }
        [HttpPost("create-campus-manager")]
        [Authorize(Roles = "Campus Manager, Admin")]
        public async Task<IActionResult> CreateManagerAsync(UserDTO userDTO)
        {
            try
            {
                var user = await _userService.CreateManagerAsync(userDTO);
                if (user == null)
                {
                    return BadRequest("Cannot create user!");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("update-campus-manager/{id}")]
        [Authorize(Roles = "Campus Manager, Admin")]
        public async Task<IActionResult> UpdateManagerAsync(UserUpdateDTO userDTO, Guid id)
        {
            try
            {
                var user = await _userService.UpdateManagerAsync(userDTO, id);
                if (user == null)
                {
                    return BadRequest("Cannot update user!");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get-list-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListUserAsync(int page, int pageSize)
        {
            try
            {
                var response =  _userService.GetListUser(page, pageSize);
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any user");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("get-user-detail/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDetailAsync(Guid id)
        {
            try
            {
                var response = await _userService.GetUserDetailAsync(id);
                if (response == null)
                {
                    return NotFound(new { message = "User not found!" });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("ban/unban-users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatusAsync(Guid id)
        {
            try
            {
               
                var response = await _userService.UpdateUserStatusAsync(id);
                if (response == null)
                {
                    return NotFound(new { message = "User not found!" });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("get-list-implementer/{eventId}")]
        [Authorize(Roles = "Event Organizer, Campus Manager, Implementer")]
        public async Task<IActionResult> GetListImplementer(int eventId, int page, int pageSize)
        {
          
            try
            {
                var response =  _userService.GetListImplementer(eventId, page, pageSize);
                if (response.TotalCount == 0)
                {
                    return NotFound("Cannot found any implementer");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin, Event Organizer, Campus Manager")]
        public async Task<IActionResult> SearchUsers(string input)
        {
            var campusId = int.Parse(User.FindFirst("campusId")?.Value);
            if (string.IsNullOrWhiteSpace(input) || campusId == null)
            {
                return BadRequest("Input và CampusName không được để trống.");
            }

            var users = await _userService.GetUserByNameOrEmailAsync(input, campusId);
            if (users.TotalCount==0)
            {
                return NotFound("Không tìm thấy người dùng phù hợp.");
            }

            return Ok(users);
        }
        [HttpPost("event-organizer")]
        [Authorize(Roles = "Campus Manager")]
        public async Task<IActionResult> CreateEventOrganizer(UserDTO userDTO)
        {
            try
            {
                var response = await _userService.CreateEventOrganizer(userDTO);
                if (response.Status!=200)
                {
                    return StatusCode(response.Status, response.Message);
                }
                var createdUser = response.Result as UserListDTO;
                UserRoleDTO roleDTO = new UserRoleDTO
                {
                    Id = 0,
                    RoleId = 3,
                    UserId = createdUser.Id
                };
                var role = await _userService.AddUserRole(roleDTO);
                if (role == null || role.Id == 0)
                {
                    return BadRequest("Cannot add user role!");
                }
                return StatusCode(response.Status, response.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("event-organizer")]
        [Authorize(Roles = "Campus Manager")]
        public async Task<IActionResult> UpdateEventOrganizer(UserDTO userDTO)
        {
            try
            {
                var response = await _userService.UpdateEventOrganizer(userDTO);
                return StatusCode(response.Status, response.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update-eog-role")]
        [Authorize(Roles = "Campus Manager, Admin")]
        public async Task<IActionResult> UpdateEventOrganizerRole(Guid userId, int roleId)
        {
            try
            {
                var response = await _userService.UpdateEOGRole(userId,roleId);
                if (!response)
                {
                    return NotFound("Not found any Event Organizer with id " + userId);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("event-organizer")]
        [Authorize(Roles = "Campus Manager, Admin")]
        public async Task<IActionResult> GetEventOrganizer(int page,int pageSize)
        {
            try
            {
                var campusId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "campusId").Value);
                var response = await _userService.GetEventOrganizer(page,pageSize,campusId);
                if (response.Items.Count() == 0)
                {
                    return NotFound("Cannot found any event organizer!");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-manager-role")]
        [Authorize(Roles = "Campus Manager, Admin")]
        public async Task<IActionResult> UpdateCampusManagerRole(Guid userId, int roleId)
        {
            try
            {
                var response = await _userService.UpdateCampusManagerRole(userId, roleId);
                if (!response)
                {
                    return NotFound("Not found any Campus Manager with id " + userId);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("campus-manager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCampusManager(int page, int pageSize)
        {
            try
            {
                //var campusId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "campusId").Value);
                var response = await _userService.GetCampusManager(page, pageSize);
                if (response.Items.Count() == 0)
                {
                    return NotFound("Cannot found any campus manager!");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search/v2")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUsersForAdmin(int page, int pageSize, string? input, string? roleName, int?campusId)
        {
            var users = await _userService.SearchUser(page, pageSize, input, roleName, campusId);
            if (users.TotalCount == 0)
            {
                return NotFound("Không tìm thấy người dùng phù hợp.");
            }

            return Ok(users);
        }

        [HttpPost("import")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTO>> ImportUsers(int campusId, IFormFile excelFile)
        {
            var response = await _userService.ImportUsersAsync(campusId, excelFile);
            return StatusCode(response.Status, response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized("Không thể xác định người dùng.");
                }

                var result = await _userService.ChangePasswordAsync(userId, request);
                return Ok(new { Message = "Đổi mật khẩu thành công." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("get-implementer-and-spectator")]
        [Authorize(Roles = "Admin, Campus Manager")]
        public async Task<IActionResult> GetImplementerAndSpectator(int page, int pageSize, string? input)
        {
            var campusId = int.Parse(User.FindFirst("campusId")?.Value);
            var users = await _userService.GetSpectatorAndImplementer(page, pageSize, input, campusId);
            if (users.TotalCount == 0)
            {
                return NotFound("Không tìm thấy người dùng phù hợp.");
            }

            return Ok(users);
        }
        [HttpGet("set-event-organizer")]
        [Authorize(Roles = "Admin, Campus Manager")]
        public async Task<IActionResult> SetEventOrganizer(Guid id)
        {
            var response = await _userService.SetRoleEOG(id);

            return StatusCode(response.Status, response);
        }
    }
}
