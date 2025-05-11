using System.ComponentModel.DataAnnotations;

namespace Planify_BackEnd.DTOs.SubTasks
{
    public class SubTaskUpdateRequestDTO
    {
        [Required]
        public string SubTaskName { get; set; }
        public string? SubTaskDescription { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
        public decimal AmountBudget { get; set; }
        public int Status { get; set; } 
        public SubTaskUpdateRequestDTO() { }
    }
}
