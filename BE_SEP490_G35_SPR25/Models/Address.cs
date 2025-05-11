using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Address
{
    public int Id { get; set; }

    public int WardId { get; set; }

    public string AddressDetail { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual Ward Ward { get; set; } = null!;
}
