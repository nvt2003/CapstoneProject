using System.ComponentModel.DataAnnotations;

namespace Planify_BackEnd.DTOs.SubTasks
{
    public class SubTaskResponseDTO
    {
        public int Id { get; set; }
        public string SubTaskName { get; set; }

        public string SubTaskDescription { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? Deadline { get; set; }

        public decimal AmountBudget { get; set; }

        public int Status { get; set; }

        public int TaskId { get; set; }
        public Guid CreateBy { get; set; }
        public string? TaskName {  get; set; }
        public string? EventTitle {  get; set; }

    }
}
