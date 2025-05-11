using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.FavouriteEvents;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.FavouriteEvents;

namespace Planify_BackEnd.Services.FavouriteEvents
{
    public class FavouriteEventService : IFavouriteEventService
    {
        private readonly IFavouriteEventRepository _favouriteEventRepository;
        public FavouriteEventService(IFavouriteEventRepository favouriteEventRepository)
        {
            _favouriteEventRepository = favouriteEventRepository;
        }

        public PageResultDTO<FavouriteEventGetDTO> GetAllFavouriteEventsAsync(int page, int pageSize,Guid  spectatorId)
        {
            try
            {
                var f = _favouriteEventRepository.GetAllFavouriteEventsAsync(page, pageSize, spectatorId);
                if (f.TotalCount == 0)

                    return new PageResultDTO<FavouriteEventGetDTO>(new List<FavouriteEventGetDTO>(), 0, page, pageSize);
                List<FavouriteEventGetDTO> fevents = new List<FavouriteEventGetDTO>();
                foreach (var item in f.Items)
                {
                    FavouriteEventGetDTO a = new FavouriteEventGetDTO
                    {
                        Id = item.Id,
                        EventId = item.EventId,
                        UserId = item.UserId,

                    };
                    fevents.Add(a);

                }
                return new PageResultDTO<FavouriteEventGetDTO>(fevents, f.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public async Task<ResponseDTO> CreateFavouriteEventAsync(int eventId, Guid spectatorId)
        {
            try
            {
                var favouriteEvent = new FavouriteEvent
                {
                    EventId = eventId,
                    UserId = spectatorId,
                };
                var result = await _favouriteEventRepository.CreateFavouriteEventAsync(favouriteEvent);
                return new ResponseDTO(201, "Favourire event create succesfully!", favouriteEvent);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "An error occurred while creating the favourite event.", ex.Message);
            }


        }
        public async Task<ResponseDTO> DeleteFavouriteEventAsync(int eventId, Guid spectatorId)
        {
            try
            {
                var isDeleted = await _favouriteEventRepository.DeleteFavouriteEventAsync(eventId, spectatorId);
                if (!isDeleted)
                {
                    return new ResponseDTO(404, "Favourite event not found or already deleted.", null);
                }

                return new ResponseDTO(200, "Favourite event deleted successfully!", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while deleting favourite event!", ex.Message);
            }
        }

        public PageResultDTO<FavouriteEventVM> GetFavouriteEventsByUserId(int page, int pageSize, Guid userId, int campusId)
        {
            try
            {
                var pageResult = _favouriteEventRepository.GetFavouriteEventsByUserId(page,pageSize,userId, campusId);
                return pageResult;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
