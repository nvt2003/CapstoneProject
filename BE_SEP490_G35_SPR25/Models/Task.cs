using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Task
{
    public int Id { get; set; }

    public Guid CreateBy { get; set; }

    public string TaskName { get; set; } = null!;

    public string TaskDescription { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? Deadline { get; set; }

    public decimal AmountBudget { get; set; }

    public int Status { get; set; }

    public int? EventId { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Event? Event { get; set; }

    public virtual ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
}
