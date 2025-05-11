using Planify_BackEnd.DTOs.Users;

namespace Planify_BackEnd.DTOs.JoinedProjects
{
    public class JoinedProjectVM
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public Guid UserId { get; set; }

        public DateTime TimeJoinProject { get; set; }

        public DateTime? TimeOutProject { get; set; }

        public UserNameVM User { get; set; } = null!;
    }
}
