using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Activity
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string Content { get; set; } = null!;

    public string? Name { get; set; }

    public virtual Event Event { get; set; } = null!;
}
