namespace Planify_BackEnd.DTOs.SendRequests
{
    public class SendRequestWithEventDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public Guid? ManagerId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventStartTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public int? requestStatus { get; set; }
    }
}
