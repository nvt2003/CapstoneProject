namespace Planify_BackEnd.DTOs.SendRequests
{
    public class SendRequestDTO
    {
        public int EventId { get; set; }
    }

    public class GetSendRequestDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = null!;
        public Guid? ManagerId { get; set; }
        public string Reason { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }
        public int? requestStatus { get; set; }
    }
}
