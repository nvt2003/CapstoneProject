namespace Planify_BackEnd.DTOs.Dashboards
{
    public class CategoryUsageDTO
    {
        public int CategoryEventId { get; set; }
        public string CategoryEventName { get; set; } = null!;
        public int TotalUsed { get; set; }
        public decimal? Percentage { get; set; }
    }
}
