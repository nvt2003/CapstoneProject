namespace Planify_BackEnd.DTOs.Reports
{
    public class ReportCreateDTO
    {
        public int Id { get; set; }

        public Guid SendFrom { get; set; }

        public int TaskId { get; set; }

        public string Reason { get; set; } = null!;

        public Guid SendTo { get; set; }

        public DateTime SendTime { get; set; }

        public int Status { get; set; }
    }
}
