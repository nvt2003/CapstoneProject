using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.SubTasks
{
    public class SubTaskVM
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public Guid CreateBy { get; set; }

        public string SubTaskName { get; set; }

        public string SubTaskDescription { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? Deadline { get; set; }

        public decimal AmountBudget { get; set; }

        public int Status { get; set; }

        public UserNameVM CreateByNavigation { get; set; }
    }
}
