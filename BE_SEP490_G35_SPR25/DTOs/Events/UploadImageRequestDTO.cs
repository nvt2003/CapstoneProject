using System.ComponentModel.DataAnnotations;

namespace Planify_BackEnd.DTOs.Events
{
    public class UploadImageRequestDTO
    {
        [Required]
        public int EventId { get; set; }
        public List<IFormFile>? EventMediaFiles { get; set; }
    }
}
