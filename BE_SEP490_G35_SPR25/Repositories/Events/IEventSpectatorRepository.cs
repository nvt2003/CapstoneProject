using Google.Apis.Drive.v3.Data;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Events
{
    public interface IEventSpectatorRepository
    {
        PageResultDTO<Event> GetEvents(int page, int pageSize, Guid userId, int campusId);
        Event GetEventById(int id,Guid userId);
        PageResultDTO<Event> SearchEvent(int page, int pageSize, string? name, DateTime? startDate, DateTime? endDate, string? placed,int?categoryId, Guid userId, int campusId);
    }
}
