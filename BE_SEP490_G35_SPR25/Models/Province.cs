using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class Province
{
    public int Id { get; set; }

    public string ProvinceName { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
