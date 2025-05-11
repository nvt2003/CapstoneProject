using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.JoinedProjects
{
    public class JoinedProjectDTO
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public Guid UserId { get; set; }

        public DateTime TimeJoinProject { get; set; }

        public DateTime? TimeOutProject { get; set; }

        public int RoleId { get; set; }

        //Event
        public string EventTitle { get; set; } = null!;

        public string EventDescription { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public decimal AmountBudget { get; set; }

        public int IsPublic { get; set; }

        public DateTime? TimePublic { get; set; }

        public int Status { get; set; }

        public Guid? ManagerId { get; set; }

        public int CampusId { get; set; }

        public int CategoryEventId { get; set; }
        public string? CategoryName { get; set; }

        public string Placed { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdateBy { get; set; }

    }
}
