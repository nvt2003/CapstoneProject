using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Participant
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public Guid UserId { get; set; }

    public DateTime RegistrationTime { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
