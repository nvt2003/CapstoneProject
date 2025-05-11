using Planify_BackEnd.DTOs.Users;

namespace Planify_BackEnd.DTOs
{
    public class EventDetailDto
    {
        public int Id { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal AmountBudget { get; set; }
        public int IsPublic { get; set; }
        public DateTime? TimePublic { get; set; }
        public int Status { get; set; }
        public string Placed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? MeasuringSuccess { get; set; }
        public string? Goals { get; set; }
        public string? MonitoringProcess { get; set; }
        public int? SizeParticipants { get; set; }
        public string? PromotionalPlan { get; set; }

        public string? TargetAudience { get; set; }

        public string? SloganEvent { get; set; }
        public string CampusName { get; set; }
        public int CategoryEventId { get; set; }
        public string CategoryEventName { get; set; }
        public UserDto CreatedBy { get; set; }
        public UserDto? Manager { get; set; }
        public UserDto? UpdatedBy { get; set; }
        public int? RequestId { get; set; }
        public List<EventMediaDto> EventMedia { get; set; }
        public List<FavouriteEventDto> FavouriteEvents { get; set; }
        public List<JoinProjectDto> JoinProjects { get; set; }
        public List<RiskDto> Risks { get; set; }
        public List<ActivityDto> Activities { get; set; }
        public List<TaskDetailDto> Tasks { get; set; }
        public List<CostBreakdownDetailDto> CostBreakdowns { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class EventMediaDto
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; }
    }

    public class FavouriteEventDto
    {
        public Guid? UserId { get; set; }
        public string UserFullName { get; set; }
    }

    public class JoinProjectDto
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public DateTime TimeJoinProject { get; set; }
        public DateTime? TimeOutProject { get; set; }
    }

    public class RiskDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Reason { get; set; }
        public string? Solution { get; set; }
        public string? Description { get; set; }
    }

    public class TaskDetailDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public decimal AmountBudget { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto CreatedBy { get; set; }
        public int Status { get; set; }
        public List<SubTaskDetailDto> SubTasks { get; set; }
    }

    public class SubTaskDetailDto
    {
        public int Id { get; set; }
        public string SubTaskName { get; set; }
        public string? SubTaskDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? Deadline { get; set; }
        public decimal AmountBudget { get; set; }
        public int Status { get; set; }
        public UserDto CreatedBy { get; set; }
        public List<UserNameVM> JoinSubTask { get; set; }
    }
    public class CostBreakdownDetailDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceByOne { get; set; }
        public decimal? TotalCost => Quantity * PriceByOne;
    }
    public class ActivityDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}