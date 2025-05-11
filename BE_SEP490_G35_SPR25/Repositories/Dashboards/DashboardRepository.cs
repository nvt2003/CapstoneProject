using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs.Dashboards;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Dashboards
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PlanifyContext _context;
        public DashboardRepository(PlanifyContext context)
        {
            _context = context;
        }
        public async Task<List<StatisticsByMonthDTO>> GetMonthlyStatsAsync(int year)
        {
            var events = await _context.Events
                .Where(e => e.Status == 2 &&
                            e.EndTime < DateTime.Now &&
                            e.StartTime.Year == year)
                .ToListAsync();

            var grouped = events
                .GroupBy(e => e.StartTime.Month)
                .ToDictionary(g => g.Key, g => new
                {
                    TotalEvents = g.Count(),
                    TotalParticipants = g.Sum(e => e.Participants.Count)
                });

            var result = Enumerable.Range(1, 12)
                .Select(month => new StatisticsByMonthDTO
                {
                    Month = $"{year}-{month:D2}",
                    TotalEvents = grouped.ContainsKey(month) ? grouped[month].TotalEvents : 0,
                    TotalParticipants = grouped.ContainsKey(month) ? grouped[month].TotalParticipants : 0
                })
                .OrderBy(x => x.Month)
                .ToList();

            return result;
        }


        public async Task<List<CategoryUsageDTO>> GetUsedCategoriesAsync()
        {
            var result = await _context.CategoryEvents
                .Where(c => c.Status == 1)
                .Select(c => new CategoryUsageDTO
                {
                    CategoryEventId = c.Id,
                    CategoryEventName = c.CategoryEventName,
                    TotalUsed = _context.Events
                        .Count(e => e.Status == 2 && e.CategoryEventId == c.Id)
                })
                .OrderByDescending(x => x.TotalUsed)
                .ToListAsync();

            return result;
        }

        public async Task<List<RecentEventDTO>> GetLatestEventsAsync(int campusId)
        {
            try
            {
                return await _context.Events
                    .Where(e => e.Status == 2 &&
                    e.CampusId == campusId)
                    .OrderByDescending(e => e.StartTime)
                    .Take(5)
                    .Select(e => new RecentEventDTO
                    {
                        Id = e.Id,
                        EventTitle = e.EventTitle,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        Status = e.Status,
                        EventDescription = e.EventDescription,
                        IsPublic = e.IsPublic,
                        CampusId = e.CampusId,
                        CategoryEventId = e.CategoryEventId,
                        Placed = e.Placed,
                        MeasuringSuccess = e.MeasuringSuccess,
                        Goals = e.Goals,
                        MonitoringProcess = e.MonitoringProcess,
                        SizeParticipants = e.SizeParticipants,
                        CreatedAt = e.CreatedAt,
                        AmountBudget = e.AmountBudget,
                        CreateBy = e.CreateBy,
                        TimePublic = e.TimePublic,
                        ManagerId = e.ManagerId,

                    })
                    .ToListAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<TopEventByParticipantsDTO>> GetTopEventsByParticipantsAsync()
        {
            return await _context.Events
                .Include(e=>e.CategoryEvent)
                .Where(e => e.Status == 1 || e.Status == 2) 
                .OrderByDescending(e => e.Participants.Count)
                .Take(10)
                .Select(e => new TopEventByParticipantsDTO
                {
                    Id = e.Id,
                    EventTitle = e.EventTitle,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    AmountBudget =e.AmountBudget,
                    TotalParticipants = e.Participants.Count,
                    CategoryEventName = e.CategoryEvent.CategoryEventName
                })
                .ToListAsync();
        }
        public async Task<List<PercentEventByCampus>> GetPercentEventsByCampus()
        {
            try
            {
                return await _context.Campuses
                            .GroupJoin(
                                _context.Events.Where(e => e.Status == 2),
                                campus => campus.Id,
                                ev => ev.CampusId,
                                (campus, events) => new PercentEventByCampus
                                {
                                    CampusName = campus.CampusName,
                                    TotalEvent = events.Count()
                                }
                            )
                            .ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }


}
