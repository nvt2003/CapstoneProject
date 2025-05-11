using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs;

namespace Planify_BackEnd.Services.Events
{
    public interface IEventSpectatorService
    {
        EventVMSpectator GetEventById(int id, Guid userId);
        PageResultDTO<EventBasicVMSpectator> GetEvents(int page, int pageSize, Guid userId, int campusId);
        PageResultDTO<EventBasicVMSpectator> SearchEvent(int page, int pageSize, string? name, DateTime? startDate, DateTime? endDate, string? placed, int? categoryId, Guid userId, int campusId);
    }
}
