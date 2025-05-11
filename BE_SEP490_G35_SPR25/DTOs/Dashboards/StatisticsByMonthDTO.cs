namespace Planify_BackEnd.DTOs.Dashboards
{
    public class StatisticsByMonthDTO
    {
        public string Month { get; set; } = null!;
        public int TotalEvents { get; set; }
        public int TotalParticipants { get; set; }
    }
}
