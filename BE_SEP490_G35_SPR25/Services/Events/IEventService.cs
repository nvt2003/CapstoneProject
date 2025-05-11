using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Services.Events
{
    public interface IEventService
    {
        PageResultDTO<EventGetListResponseDTO> GetAllEvent(int campusId, int page, int pageSize, Guid userId);
        Task<ResponseDTO> CreateEventAsync(EventCreateRequestDTO eventDTO, Guid organizerId);
        Task<ResponseDTO> UploadImageAsync(UploadImageRequestDTO imageDTO);
        Task<ResponseDTO> GetEventDetailAsync(int eventId);
        Task<ResponseDTO> UpdateEventAsync(EventUpdateDTO e);
        Task<bool> DeleteEventAsync(int eventId);
        Task<PageResultDTO<EventGetListResponseDTO>> SearchEventAsync(int page, int pageSize,
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

        //Task<ResponseDTO> CreateSaveDraft(EventCreateRequestDTO eventDTO, Guid organizerId);
        //Task<ResponseDTO> UpdateSaveDraft(EventUpdateDTO eventDTO);
        //Task<ResponseDTO> GetSaveDraft(Guid createBy);
        Task<ResponseDTO> DeleteImagesAsync(DeleteImagesRequestDTO request);
        Task<bool> EventIncomingNotification(Guid userId, string? role);
        Task<PageResultDTO<EventGetListResponseDTO>> MyEvent(int page, int pageSize, Guid createBy, int campusId);
    }
}
