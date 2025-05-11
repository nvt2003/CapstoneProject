using System.ComponentModel.DataAnnotations;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.SubTasks
{
    public class SubTaskCreateRequestDTO
    {
        [Required]
        public string SubTaskName { get; set; }

        public string? SubTaskDescription { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime Deadline { get; set; }

        public decimal AmountBudget { get; set; }
        [Required]
        public int TaskId { get; set; }
        public  SubTaskCreateRequestDTO() { }

    }
}
