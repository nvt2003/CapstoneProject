namespace Planify_BackEnd.DTOs.Events
{
    public class DeleteImagesRequestDTO
    {
        public int EventId { get; set; }
        public List<int> MediaIds { get; set; } = new List<int>();
    }
}
