using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.JoinedProjects;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Services.JoinProjects
{
    public interface IJoinProjectService
    {
        PageResultDTO<JoinedProjectDTO> JoiningEvents(int page, int pageSize, Guid userId);
        PageResultDTO<JoinedProjectDTO> AttendedEvents(int page, int pageSize, Guid userId);
        Task<bool> DeleteImplementerFromEvent(Guid userId, int eventId);
        Task<ResponseDTO> AddImplementersToEventAsync(AddImplementersToEventDTO request);
        Task<PageResultDTO<JoinedProjectVM>> SearchImplementerJoinedEvent(int page, int pageSize, int? eventId, string?email, string? name);
    }
}
