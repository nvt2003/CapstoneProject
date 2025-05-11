using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class FavouriteEvent
{
    public int Id { get; set; }

    public int? EventId { get; set; }

    public Guid? UserId { get; set; }

    public virtual Event? Event { get; set; }

    public virtual User? User { get; set; }
}
