using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.JoinGroups
{
    public interface IJoinProjectRepository
    {
        PageResultDTO<JoinProject> JoiningEvents(Guid userId, int page, int pageSize);
        Task<bool> DeleteImplementerFromEvent(Guid userId, int eventId);
        Task<bool> AddImplementersToProject(List<Guid> implementerIds, int eventId);
        Task<bool> AddImplementerToProject(Guid implementerId, int eventId);
        Task<bool> AddRoleImplementer(Guid implementerId);
        Task<bool> AddRoleImplementers(List<Guid> implementerIds);
        Task<bool> EventExistsAsync(int eventId);
        Task<List<Guid>> GetInvalidUserIdsAsync(List<Guid> userIds);
        Task<List<Guid>> GetExistingImplementerIdsAsync(int eventId);
        PageResultDTO<JoinProject> AttendedEvents(int page, int pageSize, Guid userId);
        Task<PageResultDTO<JoinProject>> SearchImplementerJoinedEvent(int page, int pageSize, 
            int? eventId, string? email, string? name);
        Task<List<Guid>> GetUserIdJoinedEvent(int eventId);
        Task<bool> IsImplementerInProject(Guid implementerId, int eventId);
    }
}
