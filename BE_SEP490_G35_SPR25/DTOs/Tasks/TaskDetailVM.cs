using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.DTOs.Groups;
using Planify_BackEnd.Models;
using Planify_BackEnd.DTOs.SubTasks;

namespace Planify_BackEnd.DTOs.Tasks
{
    public class TaskDetailVM
    {
        public int Id { get; set; }

        public Guid CreateBy { get; set; }

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? Deadline { get; set; }

        public decimal AmountBudget { get; set; }

        public double Progress { get; set; }

        public int Status { get; set; }

        public int? EventId { get; set; }

        public UserNameVM CreateByNavigation { get; set; }

        public ICollection<SubTaskVM> SubTasks { get; set; }
    }
}
