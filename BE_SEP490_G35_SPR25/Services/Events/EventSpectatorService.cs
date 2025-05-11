using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Activity;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs.Medias;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Events;

namespace Planify_BackEnd.Services.Events
{
    public class EventSpectatorService : IEventSpectatorService
    {
        private IEventSpectatorRepository _repository;
        public EventSpectatorService(IEventSpectatorRepository repository)
        {
            _repository = repository;
        }
        public EventVMSpectator GetEventById(int id, Guid userId)
        {
            try
            {

                var e = _repository.GetEventById(id,userId);
                EventVMSpectator eventVM = new EventVMSpectator
                {
                    Id = e.Id,
                    EventTitle = e.EventTitle,
                    EventDescription = e.EventDescription,
                    CampusId = e.CampusId,
                    CampusDTO = e.Campus == null ? new DTOs.Campus.CampusDTO() : new DTOs.Campus.CampusDTO
                    {
                        Id = e.Campus.Id,
                        CampusName = e.Campus.CampusName
                    },
                    Status = e.Status,
                    CategoryEventId = e.CategoryEventId,
                    CategoryViewModel = e.CategoryEvent == null ? new DTOs.Categories.CategoryViewModel() : new DTOs.Categories.CategoryViewModel
                    {
                        Id = e.CategoryEvent.Id,
                        CategoryEventName = e.CategoryEvent.CategoryEventName
                    },
                    CreatedAt = e.CreatedAt,
                    IsPublic = e.IsPublic,
                    Placed = e.Placed,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    TimePublic = e.TimePublic,
                    SloganEvent = e.SloganEvent,
                    TargetAudience = e.TargetAudience,
                    EventMedias = e.EventMedia == null ? new List<EventMediumViewMediaModel>() : e.EventMedia.Select(em => new DTOs.Medias.EventMediumViewMediaModel
                    {
                        Id = em.Id,
                        EventId = em.EventId,
                        MediaId = em.MediaId,
                        Status = em.Status,
                        MediaDTO = new DTOs.Medias.MediaItemDTO
                        {
                            Id = em.Media.Id,
                            MediaUrl = em.Media.MediaUrl
                        }
                    }).ToList(),
                    Activities = e.Activities==null?
                    new List<ActivityDTO>():
                    e.Activities.Select(a => new ActivityDTO
                    {
                        Id = a.Id,
                        EventId=a.EventId,
                        Content=a.Content,
                        Name = a.Name
                    }).ToList(),
                    isFavorite = e.FavouriteEvents.Count()!=0
                };
                return eventVM;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public PageResultDTO<EventBasicVMSpectator> GetEvents(int page, int pageSize, Guid userId, int campusId)
        {
            try
            {

                PageResultDTO<Event> events = _repository.GetEvents(page, pageSize, userId, campusId);
                if (events.TotalCount == 0) 
                    return new PageResultDTO<EventBasicVMSpectator>(new List<EventBasicVMSpectator>(), 0, page, pageSize);
                List<EventBasicVMSpectator> eventBasicVMs = new List<EventBasicVMSpectator>();
                foreach (var item in events.Items)
                {
                    EventBasicVMSpectator eventBasicVM = new EventBasicVMSpectator
                    {
                        Id = item.Id,
                        CampusId = item.CampusId,
                        CampusDTO = item.Campus == null ? null : new DTOs.Campus.CampusDTO
                        {
                            Id = item.Campus.Id,
                            CampusName = item.Campus.CampusName,
                            Status = item.Campus.Status
                        },
                        CategoryEventId = item.CategoryEventId,
                        CategoryViewModel = item.CategoryEvent == null ? null : new DTOs.Categories.CategoryViewModel
                        {
                            Id = item.CategoryEvent.Id,
                            CategoryEventName = item.CategoryEvent.CategoryEventName
                        },
                        Status = item.Status,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        EventDescription = item.EventDescription,
                        EventTitle = item.EventTitle,
                        IsPublic = item.IsPublic,
                        Placed = item.Placed,
                        isFavorite = item.FavouriteEvents.Count != 0,
                        EventMedias = item.EventMedia == null ? null : item.EventMedia.Select(em => new DTOs.Medias.EventMediumViewMediaModel
                        {
                            Id = em.Id,
                            EventId = em.Id,
                            MediaId = em.Id,
                            Status = em.Status,
                            MediaDTO = new DTOs.Medias.MediaItemDTO
                            {
                                Id = em.Media.Id,
                                MediaUrl = em.Media.MediaUrl
                            },
                        }).ToList()

                    };
                    eventBasicVMs.Add(eventBasicVM);
                }
                return new PageResultDTO<EventBasicVMSpectator>(eventBasicVMs, events.TotalCount,page,pageSize);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public PageResultDTO<EventBasicVMSpectator> SearchEvent(int page, int pageSize, string?name, DateTime? startDate, DateTime? endDate, string? placed, int? categoryId, Guid userId, int campusId)
        {
            try
            {
                if (name == null) name = "";
                if (placed == null) placed = "";
                PageResultDTO<Event> resultEvents = _repository.SearchEvent(page, pageSize, name, startDate, endDate,placed,categoryId,userId,campusId);
                List<EventBasicVMSpectator> eventBasicVMs = new List<EventBasicVMSpectator>();
                foreach (var item in resultEvents.Items)
                {
                    EventBasicVMSpectator eventBasicVM = new EventBasicVMSpectator
                    {
                        Id = item.Id,
                        CampusId = item.CampusId,
                        CampusDTO = item.Campus == null ? null : new DTOs.Campus.CampusDTO
                        {
                            Id = item.Campus.Id,
                            CampusName = item.Campus.CampusName,
                            Status = item.Campus.Status
                        },
                        CategoryEventId = item.CategoryEventId,
                        CategoryViewModel = item.CategoryEvent == null ? null : new DTOs.Categories.CategoryViewModel
                        {
                            Id = item.CategoryEvent.Id,
                            CategoryEventName = item.CategoryEvent.CategoryEventName
                        },
                        Status = item.Status,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        EventDescription = item.EventDescription,
                        EventTitle = item.EventTitle,
                        IsPublic = item.IsPublic,
                        Placed = item.Placed,
                        isFavorite = item.FavouriteEvents.Count!=0,
                        EventMedias = item.EventMedia == null ? null : item.EventMedia.Select(em => new DTOs.Medias.EventMediumViewMediaModel
                        {
                            Id = em.Id,
                            EventId = em.Id,
                            MediaId = em.Id,
                            Status = em.Status,
                            MediaDTO = new DTOs.Medias.MediaItemDTO
                            {
                                Id = em.Media.Id,
                                MediaUrl = em.Media.MediaUrl
                            },
                        }).ToList()

                    };
                    eventBasicVMs.Add(eventBasicVM);
                }
                return new PageResultDTO<EventBasicVMSpectator>(eventBasicVMs,resultEvents.TotalCount,page,pageSize);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
