using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.FavouriteEvents;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.FavouriteEvents
{
    public interface IFavouriteEventRepository
    {
        PageResultDTO<FavouriteEvent> GetAllFavouriteEventsAsync(int page, int pageSize, Guid spectatorId);
        Task<FavouriteEvent> CreateFavouriteEventAsync(FavouriteEvent favouriteEvent);
        Task<bool> DeleteFavouriteEventAsync(int eventId, Guid spectatorId);
        PageResultDTO<FavouriteEventVM> GetFavouriteEventsByUserId(int page, int pageSize, Guid userId, int campusId);
    }
}
