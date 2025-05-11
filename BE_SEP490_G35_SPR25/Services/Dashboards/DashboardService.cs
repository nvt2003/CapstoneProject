using Planify_BackEnd.DTOs.Dashboards;
using Planify_BackEnd.Repositories.Dashboards;

namespace Planify_BackEnd.Services.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ICampusRepository _campusRepository;
        public DashboardService(IDashboardRepository dashboardRepository, ICampusRepository campusRepository)
        {
            _dashboardRepository = dashboardRepository;
            _campusRepository = campusRepository;
        }
        public async Task<List<StatisticsByMonthDTO>> GetMonthlyStatsAsync(int year)
        {
            return await _dashboardRepository.GetMonthlyStatsAsync(year);
        }
        public async Task<List<CategoryUsageDTO>> GetUsedCategoriesAsync()
        {
            try
            {
                var list = await _dashboardRepository.GetUsedCategoriesAsync();
                var total = list.Sum(c => c.TotalUsed);
                foreach (var item in list)
                {
                    item.Percentage = total > 0 ? decimal.Parse(item.TotalUsed+"") * 100 / total : 0;
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<RecentEventDTO>> GetLatestEventsAsync(string campusName)
        {
            try
            {
                var campusId = await _campusRepository.GetCampusByName(campusName);
                return await _dashboardRepository.GetLatestEventsAsync(campusId.Id);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<TopEventByParticipantsDTO>> GetTopEventsByParticipantsAsync()
        {
            return await _dashboardRepository.GetTopEventsByParticipantsAsync();
        }

        public async Task<List<PercentEventByCampus>> GetPercentEventsByCampus()
        {
            try
            {
                var list = await _dashboardRepository.GetPercentEventsByCampus();
                var total = list.Sum(p => p.TotalEvent);
                foreach(var data in list)
                {
                    data.Percent = total != 0 ? decimal.Parse(data.TotalEvent+"") / total * 100 : 0;
                }
                return list;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    
}
