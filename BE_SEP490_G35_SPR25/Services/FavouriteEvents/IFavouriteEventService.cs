using Planify_BackEnd.DTOs.FavouriteEvents;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Services.FavouriteEvents
{
    public interface IFavouriteEventService
    {
        Task<ResponseDTO> CreateFavouriteEventAsync(int eventId, Guid spectatorId);
        PageResultDTO<FavouriteEventGetDTO> GetAllFavouriteEventsAsync(int page, int pageSize, Guid spectatorId);
        Task<ResponseDTO> DeleteFavouriteEventAsync(int eventId, Guid spectatorId);
        PageResultDTO<FavouriteEventVM> GetFavouriteEventsByUserId(int page, int pageSize, Guid userId, int campusId);
    }
}
