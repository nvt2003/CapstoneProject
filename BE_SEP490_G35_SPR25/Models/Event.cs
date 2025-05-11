using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Event
{
    public int Id { get; set; }

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

    public string Placed { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid CreateBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdateBy { get; set; }

    public string? MeasuringSuccess { get; set; }

    public string? Goals { get; set; }

    public string? MonitoringProcess { get; set; }

    public int? SizeParticipants { get; set; }

    public string? PromotionalPlan { get; set; }

    public string? TargetAudience { get; set; }

    public string? SloganEvent { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual Campus Campus { get; set; } = null!;

    public virtual CategoryEvent CategoryEvent { get; set; } = null!;

    public virtual ICollection<CostBreakdown> CostBreakdowns { get; set; } = new List<CostBreakdown>();

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual ICollection<EventMedium> EventMedia { get; set; } = new List<EventMedium>();

    public virtual ICollection<FavouriteEvent> FavouriteEvents { get; set; } = new List<FavouriteEvent>();

    public virtual ICollection<JoinProject> JoinProjects { get; set; } = new List<JoinProject>();

    public virtual User? Manager { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual ICollection<Risk> Risks { get; set; } = new List<Risk>();

    public virtual ICollection<SendRequest> SendRequests { get; set; } = new List<SendRequest>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual User? UpdateByNavigation { get; set; }
}
