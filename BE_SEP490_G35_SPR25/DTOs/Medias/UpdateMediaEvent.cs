namespace Planify_BackEnd.DTOs.Medias
{
    public class UpdateMediaEvent
    {
        public int EventId { get; set; }
        public List<IFormFile>? EventMediaFiles { get; set; }
        public List<int>? ListMedia { get; set; }
    }
}
