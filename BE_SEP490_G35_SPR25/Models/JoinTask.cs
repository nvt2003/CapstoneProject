using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class JoinTask
{
    public int JoinTaskId { get; set; }

    public Guid UserId { get; set; }

    public int TaskId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual SubTask Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
