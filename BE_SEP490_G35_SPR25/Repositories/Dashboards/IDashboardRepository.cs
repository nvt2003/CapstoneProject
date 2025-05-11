using Planify_BackEnd.DTOs.Dashboards;

namespace Planify_BackEnd.Repositories.Dashboards
{
    public interface IDashboardRepository
    {
        Task<List<StatisticsByMonthDTO>> GetMonthlyStatsAsync(int year);
        Task<List<CategoryUsageDTO>> GetUsedCategoriesAsync();
        Task<List<RecentEventDTO>> GetLatestEventsAsync(int campusId);
        Task<List<TopEventByParticipantsDTO>> GetTopEventsByParticipantsAsync();
        Task<List<PercentEventByCampus>> GetPercentEventsByCampus();
    }
}
