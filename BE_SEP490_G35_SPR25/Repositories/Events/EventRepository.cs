using Google.Apis.Drive.v3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;


public class EventRepository : IEventRepository
{
    private readonly PlanifyContext _context;

    public EventRepository(PlanifyContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public PageResultDTO<Event> GetAllEvent(int campusId, int page, int pageSize, Guid userId)
    {
        try
        {
            var now = DateTime.Now;

            var count = _context.Events
                .Where(e => e.Status == 2 && e.CampusId == campusId)
                .Include(e => e.EventMedia)
                .ThenInclude(em => em.Media)
                .Include(e => e.FavouriteEvents.Where(fe => fe.UserId == userId))
                .OrderBy(e => e.StartTime <= now && now <= e.EndTime ? 0 : // Running
                           e.StartTime > now ? 1 : // Not Started Yet
                           2) // Closed 
                .Count();
            if (count == 0)
                return new PageResultDTO<Event>(new List<Event>(), count, page, pageSize);

            var events = _context.Events
                .Where(e => e.Status == 2 && e.CampusId == campusId)
                .Include(e => e.EventMedia)
                .ThenInclude(em => em.Media)
                .Include(e => e.FavouriteEvents.Where(fe => fe.UserId == userId))
                .OrderBy(e => e.StartTime <= now && now <= e.EndTime ? 0 : // Running
                           e.StartTime > now ? 1 : // Not Started Yet
                           2) // Closed 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            PageResultDTO<Event> result = new PageResultDTO<Event>(events, count, page, pageSize);
             return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<Event> CreateEventAsync(Event newEvent)
    {
        try
        {
            await _context.Events.AddAsync(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public async System.Threading.Tasks.Task CreateMediaItemAsync(Medium mediaItem)
    {
        _context.Media.Add(mediaItem);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task AddEventMediaAsync(EventMedium eventMedia)
    {
        _context.EventMedia.Add(eventMedia);
        await _context.SaveChangesAsync();
    }

    public async Task<CategoryEvent> GetCategoryEventAsync(int categoryId, int campusId)
    {
        try
        {
            return await _context.CategoryEvents.FirstOrDefaultAsync(c => c.Id == categoryId && c.CampusId == campusId);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred.", ex);
        }
    }

    public async Task<EventDetailDto> GetEventDetailAsync(int eventId)
    {
        if (eventId <= 0)
        {
            throw new ArgumentException("ID sự kiện phải là số nguyên dương.", nameof(eventId));
        }

        try
        {
            // Base query with minimal includes and split query for performance
            var eventDetail = await _context.Events
                .AsSplitQuery() // Split related data into separate queries
                .Where(e => e.Id == eventId)
                .Select(e => new EventDetailDto
                {
                    Id = e.Id,
                    EventTitle = e.EventTitle,
                    EventDescription = e.EventDescription,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    AmountBudget = e.AmountBudget,
                    IsPublic = e.IsPublic,
                    TimePublic = e.TimePublic,
                    Status = e.Status,
                    Placed = e.Placed,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    MeasuringSuccess = e.MeasuringSuccess,
                    Goals = e.Goals,
                    MonitoringProcess = e.MonitoringProcess,
                    SizeParticipants = e.SizeParticipants,
                    PromotionalPlan = e.PromotionalPlan,
                    TargetAudience = e.TargetAudience,
                    SloganEvent = e.SloganEvent,
                    CampusName = e.Campus != null ? e.Campus.CampusName : null,
                    CategoryEventId = e.CategoryEventId,
                    CategoryEventName = e.CategoryEvent != null ? e.CategoryEvent.CategoryEventName : null,
                    CreatedBy = new UserDto
                    {
                        Id = e.CreateByNavigation.Id,
                        FirstName = e.CreateByNavigation.FirstName,
                        LastName = e.CreateByNavigation.LastName,
                        Email = e.CreateByNavigation.Email
                    },
                    Manager = e.Manager != null ? new UserDto
                    {
                        Id = e.Manager.Id,
                        FirstName = e.Manager.FirstName,
                        LastName = e.Manager.LastName,
                        Email = e.Manager.Email
                    } : null,
                    UpdatedBy = e.UpdateByNavigation != null ? new UserDto
                    {
                        Id = e.UpdateByNavigation.Id,
                        FirstName = e.UpdateByNavigation.FirstName,
                        LastName = e.UpdateByNavigation.LastName,
                        Email = e.UpdateByNavigation.Email
                    } : null
                })
                .FirstOrDefaultAsync();

            if (eventDetail == null)
            {
                throw new Exception($"Không tìm thấy sự kiện với ID {eventId}.");
            }

            // Load related collections separately to avoid large joins
            eventDetail.EventMedia = await _context.EventMedia
                .Where(em => em.EventId == eventId && em.Status == 1)
                .Select(em => new EventMediaDto
                {
                    Id = em.Id,
                    MediaUrl = em.Media != null ? em.Media.MediaUrl : null
                })
                .ToListAsync();

            eventDetail.FavouriteEvents = await _context.FavouriteEvents
                .Where(fe => fe.EventId == eventId)
                .Select(fe => new FavouriteEventDto
                {
                    UserId = fe.UserId,
                    UserFullName = $"{fe.User.FirstName} {fe.User.LastName}"
                })
                .ToListAsync();

            eventDetail.JoinProjects = await _context.JoinProjects
                .Where(jp => jp.EventId == eventId && jp.TimeOutProject == null)
                .Select(jp => new JoinProjectDto
                {
                    UserId = jp.UserId,
                    UserFullName = $"{jp.User.FirstName} {jp.User.LastName}",
                    TimeJoinProject = jp.TimeJoinProject
                })
                .ToListAsync();

            eventDetail.Risks = await _context.Risks
                .Where(r => r.EventId == eventId)
                .Select(r => new RiskDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Reason = r.Reason,
                    Solution = r.Solution,
                    Description = r.Description
                })
                .ToListAsync();

            eventDetail.Activities = await _context.Activities
                .Where(a => a.EventId == eventId)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    EventId = a.EventId,
                    Name = a.Name,
                    Content = a.Content
                })
                .ToListAsync();

            eventDetail.Tasks = await _context.Tasks
                .Where(t => t.EventId == eventId && (t.Status == 1 || t.Status == 0))
                .Select(t => new TaskDetailDto
                {
                    Id = t.Id,
                    TaskName = t.TaskName,
                    TaskDescription = t.TaskDescription,
                    StartTime = t.StartTime,
                    Deadline = t.Deadline,
                    AmountBudget = t.AmountBudget,
                    CreatedAt = t.CreateDate,
                    CreatedBy = new UserDto
                    {
                        Id = t.CreateByNavigation.Id,
                        FirstName = t.CreateByNavigation.FirstName,
                        LastName = t.CreateByNavigation.LastName,
                        Email = t.CreateByNavigation.Email
                    },
                    Status = t.Status,
                    SubTasks = t.SubTasks
                        .Where(st => st.Status == 1 || st.Status == 0)
                        .Select(st => new SubTaskDetailDto
                        {
                            Id = st.Id,
                            SubTaskName = st.SubTaskName,
                            SubTaskDescription = st.SubTaskDescription,
                            StartTime = st.StartTime,
                            Deadline = st.Deadline,
                            AmountBudget = st.AmountBudget,
                            CreatedBy = new UserDto
                            {
                                Id = st.CreateByNavigation.Id,
                                FirstName = st.CreateByNavigation.FirstName,
                                LastName = st.CreateByNavigation.LastName,
                                Email = st.CreateByNavigation.Email
                            },
                            Status = st.Status,
                            JoinSubTask = st.JoinTasks.Select(jt => new UserNameVM
                            {
                                Id = jt.UserId,
                                Email = jt.User.Email,
                                FirstName = jt.User.FirstName,
                                LastName = jt.User.LastName
                            }).ToList()
                        }).ToList()
                })
                .ToListAsync();

            eventDetail.CostBreakdowns = await _context.CostBreakdowns
                .Where(cb => cb.EventId == eventId)
                .Select(cb => new CostBreakdownDetailDto
                {
                    Id = cb.Id,
                    Name = cb.Name,
                    Quantity = cb.Quantity,
                    PriceByOne = cb.PriceByOne
                })
                .ToListAsync();

            eventDetail.RequestId = await _context.SendRequests
                .Where(sr => sr.EventId == eventId)
                .OrderByDescending(sr => sr.Id)
                .Select(sr => sr.Id)
                .FirstOrDefaultAsync();

            return eventDetail;
        }
        catch (Exception ex)
        {
            throw new Exception($"Đã xảy ra lỗi khi lấy chi tiết sự kiện với eventId {eventId}. Chi tiết: {ex.Message}, StackTrace: {ex.StackTrace}", ex);
        }
    }

    public async Task<Event> UpdateEventAsync(Event e)
    {
        try
        {
            var updateEvent = await _context.Events.FirstOrDefaultAsync(ev=>ev.Id==e.Id);
            updateEvent.Id = e.Id;
            updateEvent.AmountBudget = e.AmountBudget;
            updateEvent.CampusId = e.CampusId;
            updateEvent.CategoryEventId = e.CategoryEventId;
            updateEvent.StartTime = e.StartTime;
            updateEvent.EndTime = e.EndTime;
            updateEvent.EventDescription = e.EventDescription;
            updateEvent.EventTitle = e.EventTitle;
            updateEvent.Placed = e.Placed;
            updateEvent.Status = e.Status;
            updateEvent.TimePublic = e.TimePublic;
            updateEvent.IsPublic = e.IsPublic;
            updateEvent.MeasuringSuccess = e.MeasuringSuccess;
            updateEvent.Goals = e.Goals;
            updateEvent.MonitoringProcess = e.MonitoringProcess;
            updateEvent.SizeParticipants = e.SizeParticipants;
            updateEvent.PromotionalPlan = e.PromotionalPlan;
            updateEvent.TargetAudience = e.TargetAudience;
            updateEvent.SloganEvent = e.SloganEvent;
            updateEvent.UpdateBy = e.UpdateBy;
            updateEvent.UpdatedAt = DateTime.Now;
            _context.Events.Update(updateEvent);
            await _context.SaveChangesAsync();
            var updatedEvent = _context.Events
                .Include(ue=>ue.Campus)
                .Include(ue=>ue.CategoryEvent)
                .Include(ue=>ue.CreateByNavigation)
                .Include(ue => ue.UpdateByNavigation)
                .Include(ue=>ue.EventMedia).ThenInclude(em=>em.Media)
                .FirstOrDefault(ue => ue.Id == e.Id);
            return updatedEvent;
        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        try
        {
            //var ev = await _context.Events
            //    .Include(e=>e.JoinProjects)
            //    .Include(e=>e.EventMedia).ThenInclude(em=>em.Media)
            //    .Include(e=>e.Groups)
            //    .Include(e=>e.SendRequests)
            //    .FirstOrDefaultAsync(e => e.Id == eventId);
            //ev.JoinProjects = null;
            //ev.EventMedia = null;
            //ev.Groups = null;
            //ev.SendRequests = null;
            //_context.Events.Remove(ev!);
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            ev.Status = -2;
            await _context.SaveChangesAsync();
            return true;
        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PageResultDTO<Event>> SearchEventAsync(int page, int pageSize, string? title, 
        DateTime? startTime, DateTime? endTime, decimal? minBudget, decimal? maxBudget, int? isPublic, 
        int? status, int? CategoryEventId, string? placed, Guid userId, int campusId, Guid? createBy)
    {
        try
        {
            var count = _context.Events
                .Include(e => e.Campus)
                .Include(e => e.CategoryEvent)
                .Include(e => e.EventMedia).ThenInclude(em => em.Media)
                .Include(e => e.FavouriteEvents.Where(fe => fe.UserId == userId))
                .Where(e => e.CampusId==campusId &&
                    (string.IsNullOrEmpty(title) || e.EventTitle.Contains(title)) &&
                    (!isPublic.HasValue || e.IsPublic == 1) &&
                    (!startTime.HasValue || e.StartTime >= startTime) &&
                    (!endTime.HasValue || e.EndTime <= endTime) &&
                    (!minBudget.HasValue || e.AmountBudget >= minBudget) &&
                    (!maxBudget.HasValue || e.AmountBudget <= maxBudget) &&
                    (!isPublic.HasValue || e.IsPublic == isPublic) &&
                    (status.HasValue ? e.Status == status : e.Status > -1) &&
                    (!CategoryEventId.HasValue || e.CategoryEventId == CategoryEventId) &&
                    (string.IsNullOrEmpty(placed) || e.Placed.Contains(placed)) &&
                    (!createBy.HasValue || e.CreateBy == createBy)
                    ).AsEnumerable()
                    .OrderBy(e =>
                        e.StartTime <= DateTime.Now && DateTime.Now <= e.EndTime ? 0:1)
                    .ThenBy(e =>
                        e.StartTime > DateTime.Now ? 0 : 1)
                    .ThenBy(e =>
                        e.EndTime < DateTime.Now ? 0 : 1)
                    .Count();
            if (count == 0) new PageResultDTO<Event>(new List<Event>(), count, page, pageSize);
            var events = _context.Events
                .Include(e => e.Campus)
                .Include(e => e.CategoryEvent)
                .Include(e => e.EventMedia).ThenInclude(em => em.Media)
                .Include(e => e.FavouriteEvents.Where(fe => fe.UserId == userId))
                .Where(e => e.CampusId == campusId &&
                    (string.IsNullOrEmpty(title) || e.EventTitle.Contains(title)) &&
                    (!isPublic.HasValue || e.IsPublic == 1) &&
                    (!startTime.HasValue || e.StartTime >= startTime) &&
                    (!endTime.HasValue || e.EndTime <= endTime) &&
                    (!minBudget.HasValue || e.AmountBudget >= minBudget) &&
                    (!maxBudget.HasValue || e.AmountBudget <= maxBudget) &&
                    (!isPublic.HasValue || e.IsPublic == isPublic) &&
                    (status.HasValue ? e.Status == status : e.Status > -1) &&
                    (!CategoryEventId.HasValue || e.CategoryEventId == CategoryEventId) &&
                    (string.IsNullOrEmpty(placed) || e.Placed.Contains(placed)) &&
                    (!createBy.HasValue || e.CreateBy == createBy)
                ).AsEnumerable()
                .OrderBy(e =>
                    e.StartTime <= DateTime.Now && DateTime.Now <= e.EndTime ? 0 : 1)
                .ThenBy(e =>
                    e.StartTime > DateTime.Now ? 0 : 1)
                .ThenBy(e =>
                    e.EndTime < DateTime.Now ? 0 : 1)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageResultDTO<Event> result = new PageResultDTO<Event>(events, count, page, pageSize);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Event> CreateSaveDraft(Event saveEvent)
    {
        try
        {
            await _context.Events.AddAsync(saveEvent);
            await _context.SaveChangesAsync();
            return saveEvent;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<Event> UpdateSaveDraft(Event saveEvent)
    {
        try
        {
            var updateSaveDraft = await _context.Events.FirstOrDefaultAsync(ev => ev.Id == saveEvent.Id);
            updateSaveDraft.Id = saveEvent.Id;
            updateSaveDraft.AmountBudget = saveEvent.AmountBudget;
            updateSaveDraft.CampusId = saveEvent.CampusId;
            updateSaveDraft.CategoryEventId = saveEvent.CategoryEventId;
            updateSaveDraft.StartTime = saveEvent.StartTime;
            updateSaveDraft.EndTime = saveEvent.EndTime;
            updateSaveDraft.EventDescription = saveEvent.EventDescription;
            updateSaveDraft.EventTitle = saveEvent.EventTitle;
            updateSaveDraft.IsPublic = saveEvent.IsPublic;
            updateSaveDraft.Placed = saveEvent.Placed;
            updateSaveDraft.Status = saveEvent.Status;
            updateSaveDraft.TimePublic = saveEvent.TimePublic;
            updateSaveDraft.UpdateBy = saveEvent.UpdateBy;
            updateSaveDraft.UpdatedAt = DateTime.Now;
            _context.Events.Update(updateSaveDraft);
            await _context.SaveChangesAsync();
            return updateSaveDraft;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Event> GetSaveDraft(Guid createBy)
    {
        try
        {
            return _context.Events.Where(e=>e.CreateBy.Equals(createBy)&&e.Status==10).OrderBy(e=>e.CreatedAt).FirstOrDefault();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async System.Threading.Tasks.Task CreateRiskAsync(Risk risk)
    {
        _context.Risks.Add(risk);
        await _context.SaveChangesAsync();
    }
    public async System.Threading.Tasks.Task CreateActivityAsync(Activity activity)
    {
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();
    }
    public async Task<Event> GetEventByIdAsync(int eventId)
    {
        return await _context.Events
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }

    //public async System.Threading.Tasks.Task CreateCostBreakdownAsync(CostBreakdown costBreakdown)
    //{
    //    _context.CostBreakdowns.Add(costBreakdown);
    //    await _context.SaveChangesAsync();
    //}


    public async System.Threading.Tasks.Task CreateCostBreakdownAsync(CostBreakdown costBreakdown)
    {
        var sql = "INSERT INTO CostBreakdown (EventId, Name, PriceByOne, Quantity) VALUES (@p0, @p1, @p2, @p3)";

        await _context.Database.ExecuteSqlRawAsync(sql,
            costBreakdown.EventId,
            costBreakdown.Name,
            costBreakdown.PriceByOne,
            costBreakdown.Quantity);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
    public async Task<List<EventMedium>> GetEventMediaByIdsAsync(int eventId, List<int> mediaIds)
    {
        return await _context.EventMedia
            .Where(em => em.EventId == eventId && mediaIds.Contains(em.MediaId))
            .ToListAsync();
    }

    public async Task<Medium> GetMediaItemAsync(int mediaId)
    {
        return await _context.Media.FindAsync(mediaId);
    }

    public async System.Threading.Tasks.Task DeleteEventMediaListAsync(int eventId, List<int> mediaIds)
    {
        var eventMediaList = await GetEventMediaByIdsAsync(eventId, mediaIds);
        if (eventMediaList.Any())
        {
            _context.EventMedia.RemoveRange(eventMediaList);
            await _context.SaveChangesAsync();
        }
    }

    public async System.Threading.Tasks.Task DeleteMediaItemsAsync(List<int> mediaIds)
    {
        var mediaItems = await _context.Media
            .Where(m => mediaIds.Contains(m.Id))
            .ToListAsync();
        if (mediaItems.Any())
        {
            _context.Media.RemoveRange(mediaItems);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<EventIncomingNotification>> GetEventIncomings(Guid userId)
    {
        try
        {
            var now = DateTime.Now;
            var future = now.AddDays(30);
            var list = await _context.JoinProjects
                .Where(jp => jp.UserId.Equals(userId))
                .Include(jp => jp.Event)
                .Where(jp => jp.Event.StartTime >= now &&
                jp.Event.StartTime <= future &&
                jp.Event.Status != -2)
                .Select(jp => new EventIncomingNotification
                {
                    EventId = jp.EventId,
                    EventTitle = jp.Event.EventTitle,
                    StartTime = jp.Event.StartTime
                })
                .ToListAsync();
            return list;
        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<List<EventIncomingNotification>> GetEventParticipantIncomings(Guid userId)
    {
        try
        {
            var now = DateTime.Now;
            var future = now.AddDays(30);
            var list = await _context.Participants
                .Where(p => p.UserId.Equals(userId))
                .Include(p => p.Event)
                .Where(p => p.Event.StartTime >= now &&
                p.Event.StartTime <= future &&
                p.Event.Status == 2)
                .Select(jp => new EventIncomingNotification
                {
                    EventId = jp.EventId,
                    EventTitle = jp.Event.EventTitle,
                    StartTime = jp.Event.StartTime
                })
                .ToListAsync();
            return list;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<PageResultDTO<Event>> MyEvent(int page, int pageSize, Guid createBy, int campusId)
    {
        try
        {
            var count = _context.Events
                .Include(e => e.Campus)
                .Include(e => e.CategoryEvent)
                .Include(e => e.EventMedia).ThenInclude(em => em.Media)
                .Include(e => e.FavouriteEvents.Where(fe => fe.UserId == createBy))
                .Where(e => e.Status > -2 && e.CreateBy == createBy &&
                    e.CampusId == campusId)
                    .AsEnumerable()
                    .OrderBy(e =>
                        e.StartTime <= DateTime.Now && DateTime.Now <= e.EndTime ? 0 : 1)
                    .ThenBy(e =>
                        e.StartTime > DateTime.Now ? 0 : 1)
                    .ThenBy(e =>
                        e.EndTime < DateTime.Now ? 0 : 1)
                    .Count();
            if (count == 0) new PageResultDTO<Event>(new List<Event>(), count, page, pageSize);
            var events = _context.Events
                .Include(e => e.Campus)
                .Include(e => e.CategoryEvent)
                .Include(e => e.EventMedia).ThenInclude(em => em.Media)
                .Include(e => e.FavouriteEvents.Where(fe => fe.UserId == createBy))
                .Where(e => e.Status > -2 && e.CreateBy == createBy &&
                    e.CampusId == campusId)
                .AsEnumerable()
                .OrderBy(e =>
                    e.StartTime <= DateTime.Now && DateTime.Now <= e.EndTime ? 0 : 1)
                .ThenBy(e =>
                    e.StartTime > DateTime.Now ? 0 : 1)
                .ThenBy(e =>
                    e.EndTime < DateTime.Now ? 0 : 1)
                .Skip((page - 1) * pageSize).Take(pageSize).ToList();
            PageResultDTO<Event> result = new PageResultDTO<Event>(events, count, page, pageSize);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

