using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Dashboards
{
    public class RecentEventDTO
    {
        public int Id { get; set; }

        public string EventTitle { get; set; } = null!;

        public string? EventDescription { get; set; }

        public Guid? CreateBy { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? CreatedAt { get; set; }

        public decimal? AmountBudget { get; set; }

        public int? IsPublic { get; set; }

        public DateTime? TimePublic { get; set; }

        public int? Status { get; set; }

        public Guid? ManagerId { get; set; }

        public int? CampusId { get; set; }

        public int? CategoryEventId { get; set; }

        public string? Placed { get; set; }
        public string? MeasuringSuccess { get; set; }

        public string? Goals { get; set; }

        public string? MonitoringProcess { get; set; }

        public int? SizeParticipants { get; set; }
    }
}
