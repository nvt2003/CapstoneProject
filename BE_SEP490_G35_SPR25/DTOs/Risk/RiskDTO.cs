namespace Planify_BackEnd.DTOs.Risk
{
    public class RiskDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string? Reason { get; set; }
        public string? Solution { get; set; }
        public string? Description { get; set; }
    }
    public class RiskCreateDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string? Reason { get; set; }
        public string? Solution { get; set; }
        public string? Description { get; set; }
    }
    public class RiskUpdateDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string? Reason { get; set; }
        public string? Solution { get; set; }
        public string? Description { get; set; }
    }
}
