using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;

public interface IUserRepository
{
    PageResultDTO<User> GetListUser(int page, int pageSize);
    Task<User> GetUserDetailAsync(Guid userId);
    Task<bool> UpdateUserStatusAsync(Guid id);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(Guid id);
    PageResultDTO<User> GetListImplementer(int eventId, int page, int pageSize);
    Task<PageResultDTO<User>> GetUserByNameOrEmail(string input, int campusId);
    Task<User> CreateEventOrganizer(User user);
    Task<User> UpdateEventOrganizer(User user);
    Task<UserRole> AddUserRole(UserRole role);
    Task<User> CreateManagerAsync(User user);
    Task<User> UpdateManagerAsync(Guid id, UserUpdateDTO updateUser);
    Task<User> GetUserByUsernameAsync(string username);
    Task<PageResultDTO<EventOrganizerVM>> GetEventOrganizer(int page, int pageSize,int campusId);
    Task<bool> UpdateRoleEOG(Guid userId, int roleId);
    Task<bool> UpdateRoleCampusManager(Guid userId, int roleId);
    Task<PageResultDTO<EventOrganizerVM>> GetCampusManager(int page, int pageSize/*, int campusId*/);
    Task<PageResultDTO<User>> SearchUser(int page, int pageSize, string? input, string? roleName, int? campusId);
    Task<User> AddUserAsync(User user);
    Task<UserRole> AddUserRoleAsync(UserRole userRole);
    Task<Campus?> GetCampusByIdAsync(int campusId);
    Task<bool> EmailExistsAsync(string email);
    System.Threading.Tasks.Task UpdateUserAsync(User user);
    Task<PageResultDTO<User>> GetSpectatorAndImplementer(int page, int pageSize, string? input, int campusId);
}