using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Campus
{
    public int Id { get; set; }

    public string CampusName { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<CategoryEvent> CategoryEvents { get; set; } = new List<CategoryEvent>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
