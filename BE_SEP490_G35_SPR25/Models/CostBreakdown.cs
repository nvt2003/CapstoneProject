using System;
using System.Collections.Generic;

namespace Planify_BackEnd.Models;

public partial class CostBreakdown
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Quantity { get; set; }

    public decimal? PriceByOne { get; set; }

    public int? EventId { get; set; }

    public virtual Event? Event { get; set; }
}
