namespace Planify_BackEnd.DTOs.CostBreakdown
{
    public class CostBreakdownDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceByOne { get; set; }
        public int EventId { get; set; }
    }
    public class CostBreakdownUpdateDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceByOne { get; set; }
        public int EventId { get; set; }
    }
    public class CostBreakdownCreateDTO
    {
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceByOne { get; set; }
        public int EventId { get; set; }
    }
}
