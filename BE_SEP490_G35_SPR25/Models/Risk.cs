using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Risk
{
    public int Id { get; set; }

    public int? EventId { get; set; }

    public string Name { get; set; } = null!;

    public string? Reason { get; set; }

    public string? Solution { get; set; }

    public string? Description { get; set; }

    public virtual Event? Event { get; set; }
}
