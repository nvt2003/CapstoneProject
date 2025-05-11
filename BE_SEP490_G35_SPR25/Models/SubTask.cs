using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class SubTask
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public Guid CreateBy { get; set; }

    public string SubTaskName { get; set; } = null!;

    public string SubTaskDescription { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime? Deadline { get; set; }

    public decimal AmountBudget { get; set; }

    public int Status { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual ICollection<JoinTask> JoinTasks { get; set; } = new List<JoinTask>();

    public virtual Task Task { get; set; } = null!;
}
