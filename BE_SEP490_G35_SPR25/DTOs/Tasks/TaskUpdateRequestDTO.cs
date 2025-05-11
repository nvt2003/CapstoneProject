using System.ComponentModel.DataAnnotations;

namespace Planify_BackEnd.DTOs.Tasks
{
    public class TaskUpdateRequestDTO
    {
        public string TaskName { get; set; }

        public int EventId { get; set; }

        public string TaskDescription { get; set; }

   
        public DateTime StartTime { get; set; }
   
        public DateTime Deadline { get; set; }
        public decimal AmountBudget { get; set; }


        public TaskUpdateRequestDTO() { }
    }
}
