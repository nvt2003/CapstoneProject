namespace Planify_BackEnd.DTOs.Tasks
{
    public class TaskSearchResponeDTO
    {
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public int? EventId { get; set; }
        public string? EventName { get; set; }
        public decimal? AmountBudget { get; set; }
        public double? Progress { get; set; }
        public int? Status { get; set; }
    }
}
