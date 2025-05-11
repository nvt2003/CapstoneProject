using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Reports
{
    public class ReportVM
    {
        public int Id { get; set; }

        public Guid SendFrom { get; set; }

        public int TaskId { get; set; }

        public string Reason { get; set; }

        public Guid SendTo { get; set; }

        public DateTime SendTime { get; set; }

        public int Status { get; set; }

        public ICollection<ReportMediumVM> ReportMedia { get; set; } = new List<ReportMediumVM>();

        public UserNameVM SendFromNavigation { get; set; }

        public UserNameVM SendToNavigation { get; set; }

        public TaskSearchResponeDTO TaskDTO { get; set; }
    }
}
