using System.ComponentModel.DataAnnotations;

namespace Planify_BackEnd.DTOs.Reports
{
    public class UploadReportImageRequestDTO
    {
        [Required]
        public int ReportId { get; set; }
        public List<IFormFile>? ReportMediaFiles { get; set; }
    }
}
