using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.FavouriteEvents;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.FavouriteEvents
{
    public class FavouriteEventRepository : IFavouriteEventRepository
    {
        private readonly PlanifyContext _context;
        public FavouriteEventRepository(PlanifyContext context)
        {
            _context = context;
        }

        public PageResultDTO<FavouriteEvent> GetAllFavouriteEventsAsync(int page, int pageSize, Guid spectatorId)
        {
            try
            {
                var count = _context.FavouriteEvents.Count();
                if (count == 0)
                    return new PageResultDTO<FavouriteEvent>(new List<FavouriteEvent>(), count, page, pageSize);
                var f = _context.FavouriteEvents
                    .Include(f => f.Event)
                    .Include(f => f.User)
                    .Where(f => f.UserId == spectatorId)
                    .OrderByDescending(f=>f.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                PageResultDTO<FavouriteEvent> result = new PageResultDTO<FavouriteEvent>(f, count, page, pageSize);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving favourite events.", ex);
            }
        }
        public async Task<FavouriteEvent> CreateFavouriteEventAsync(FavouriteEvent favouriteEvent)
        {
            try
            {
                await _context.FavouriteEvents.AddAsync(favouriteEvent);
                await _context.SaveChangesAsync();
                return favouriteEvent;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the favourite event.", ex);
            }

        }
        public async Task<bool> DeleteFavouriteEventAsync(int eventId, Guid spectatorId)
        {
            try
            {
                var fevent = await _context.FavouriteEvents.FirstOrDefaultAsync(f => f.EventId == eventId && f.UserId == spectatorId);

                if (fevent == null)
                {
                    return false;
                }
                _context.FavouriteEvents.Remove(fevent);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public PageResultDTO<FavouriteEventVM> GetFavouriteEventsByUserId(int page, int pageSize, Guid userId, int campusId)
        {
            try
            {
                var count = _context.FavouriteEvents
                    .Include(f => f.Event).ThenInclude(e => e.EventMedia).ThenInclude(em => em.Media)
                    .Include(f => f.User)
                    .Where(f => f.UserId == userId &&
                    f.Event.CampusId==campusId)
                    .Count();
                if (count == 0) return new PageResultDTO<FavouriteEventVM>(new List<FavouriteEventVM>(), 0, page, pageSize);
                var f = _context.FavouriteEvents
                    .Include(f => f.Event).ThenInclude(e=>e.EventMedia).ThenInclude(em=>em.Media)
                    .Include(f => f.User)
                    .Where(f => f.UserId == userId &&
                    f.Event.CampusId == campusId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f=> new FavouriteEventVM
                    {
                        Id = f.Event.Id,
                        EventTitle = f.Event.EventTitle,
                        StartTime = f.Event.StartTime,
                        EndTime = f.Event.EndTime,
                        Placed = f.Event.Placed,
                        Status = f.Event.Status,
                        EventMedia = f.Event.EventMedia.Select(em=> new EventMediaDto
                        {
                            Id= em.Media.Id,
                            MediaUrl = em.Media.MediaUrl,
                        }).ToList()
                    })
                    .ToList();
                PageResultDTO<FavouriteEventVM> result = new PageResultDTO<FavouriteEventVM>(f, count, page, pageSize);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving favourite events.", ex);
            }
        }
    }
}
