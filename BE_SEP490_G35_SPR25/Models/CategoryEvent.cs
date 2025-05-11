using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class CategoryEvent
{
    public int Id { get; set; }

    public string CategoryEventName { get; set; } = null!;

    public int CampusId { get; set; }

    public int Status { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
