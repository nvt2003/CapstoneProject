using Microsoft.EntityFrameworkCore.Storage;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories
{
    public interface IEventRepository
    {
        PageResultDTO<Event> GetAllEvent(int campusId, int page, int pageSize, Guid userId);
        Task<Event> CreateEventAsync(Event newEvent);

        System.Threading.Tasks.Task CreateMediaItemAsync(Medium mediaItem);
        System.Threading.Tasks.Task AddEventMediaAsync(EventMedium eventMedia);
        Task<CategoryEvent> GetCategoryEventAsync(int categoryId, int campusId);
        Task<EventDetailDto> GetEventDetailAsync(int eventId);
        Task<Event> UpdateEventAsync(Event e);
        Task<bool> DeleteEventAsync(int eventId);
        Task<PageResultDTO<Event>>SearchEventAsync(int page,int pageSize, 
            string? title, 
            DateTime? startTime, DateTime? endTime,
            decimal? minBudget, decimal? maxBudget,
            int? isPublic,
            int? status,
            int? CategoryEventId,
            string? placed,
            Guid userId,
            int campusId,
            Guid? createBy);
        Task<Event> CreateSaveDraft(Event saveEvent);
        Task<Event> UpdateSaveDraft(Event saveEvent);
        Task<Event> GetSaveDraft(Guid createBy);
        System.Threading.Tasks.Task CreateRiskAsync(Models.Risk risk);
        System.Threading.Tasks.Task CreateActivityAsync(Activity activity);
        Task<Event> GetEventByIdAsync(int eventId);
        System.Threading.Tasks.Task CreateCostBreakdownAsync(CostBreakdown costBreakdown);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<List<EventMedium>> GetEventMediaByIdsAsync(int eventId, List<int> mediaIds);
        Task<Medium> GetMediaItemAsync(int mediaId);
        System.Threading.Tasks.Task DeleteEventMediaListAsync(int eventId, List<int> mediaIds);
        System.Threading.Tasks.Task DeleteMediaItemsAsync(List<int> mediaIds);
        Task<List<EventIncomingNotification>> GetEventIncomings(Guid userId);
        Task<List<EventIncomingNotification>> GetEventParticipantIncomings(Guid userId);
        Task<PageResultDTO<Event>> MyEvent(int page, int pageSize, Guid createBy, int campusId);
    }
}
