namespace Planify_BackEnd.DTOs.Activity
{
    public class ActivityDTO
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string? Content { get; set; }

        public string? Name { get; set; }
    }
}
