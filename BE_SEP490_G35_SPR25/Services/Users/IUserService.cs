using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Users;

namespace Planify_BackEnd.Services.Users
{
    public interface IUserservice
    {
        PageResultDTO<UserListDTO> GetListUser(int page, int pageSize);
        Task<UserDetailDTO> GetUserDetailAsync(Guid id);
        Task<ResponseDTO> UpdateUserStatusAsync(Guid id);
        PageResultDTO<UserListDTO> GetListImplementer(int eventId, int page, int pageSize);
        Task<PageResultDTO<UserListDTO>> GetUserByNameOrEmailAsync(string input, int campusId);
        Task<ResponseDTO> CreateEventOrganizer(UserDTO userDTO);
        Task<ResponseDTO> UpdateEventOrganizer(UserDTO userDTO);
        Task<UserRoleDTO> AddUserRole(UserRoleDTO roleDTO);
        Task<ResponseDTO> CreateManagerAsync(UserDTO user);
        Task<ResponseDTO> UpdateManagerAsync(UserUpdateDTO user, Guid id);
        Task<PageResultDTO<EventOrganizerVM>> GetEventOrganizer(int page, int pageSize, int campusId);
        Task<bool> UpdateEOGRole(Guid userId,int roleId);
        Task<bool> UpdateCampusManagerRole(Guid userId, int roleId);
        Task<PageResultDTO<EventOrganizerVM>> GetCampusManager(int page, int pageSize/*, int campusId*/);
        Task<PageResultDTO<UserListDTO>> SearchUser(int page, int pageSize, string? input, string? roleName, int? campusId);
        Task<ResponseDTO> ImportUsersAsync(int campusId, IFormFile excelFile);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<PageResultDTO<UserListDTO>> GetSpectatorAndImplementer(int page, int pageSize, string? input, int campusId);
        Task<ResponseDTO> SetRoleEOG(Guid userId);
    }
}
