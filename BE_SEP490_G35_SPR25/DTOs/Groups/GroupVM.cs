using Planify_BackEnd.DTOs.Implementers;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Groups
{
    public class GroupVM
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public int EventId { get; set; }

        public Guid CreateBy { get; set; }

        public decimal AmountBudget { get; set; }

        public UserNameVM CreateByNavigation { get; set; }

        public ICollection<JoinGroupVM> JoinGroups { get; set; } = new List<JoinGroupVM>();

        public virtual ICollection<TaskSearchResponeDTO> Tasks { get; set; } = new List<TaskSearchResponeDTO>();

    }
}
