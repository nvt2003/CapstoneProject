using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.DTOs.Medias;

namespace Planify_BackEnd.DTOs.Events
{
    public class EventGetListResponseDTO
    {
        public int Id { get; set; }

        public string EventTitle { get; set; } = null!;

        public string? EventDescription { get; set; }

        public Guid? CreateBy { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? TimeOfEvent { get; set; }

        public DateTime? EndOfEvent { get; set; }

        public DateTime? CreatedAt { get; set; }

        public decimal? AmountBudget { get; set; }

        public int? IsPublic { get; set; }

        public DateTime? TimePublic { get; set; }

        public int? Status { get; set; }

        public Guid? ManagerId { get; set; }

        public int? CampusId { get; set; }

        public int? CategoryEventId { get; set; }

        public string? Placed { get; set; }
        public string? MeasuringSuccess { get; set; }

        public string? Goals { get; set; }

        public string? MonitoringProcess { get; set; }

        public int? SizeParticipants { get; set; }

        public ICollection<EventMediumViewMediaModel> EventMedias { get; set; } = new List<EventMediumViewMediaModel>();

        public bool? isFavorite { get; set; }
        public string? StatusMessage
        {
            get
            {
                if (StartTime.HasValue && StartTime.Value <= DateTime.Now && EndTime.HasValue && EndTime.Value >= DateTime.Now)
                {
                    return "Đang diễn ra";
                }
                else if (StartTime.HasValue && StartTime.Value > DateTime.Now)
                {
                    return "Chưa bắt đầu";
                }
                else if (EndTime.HasValue && EndTime.Value < DateTime.Now)
                {
                    return "Đã kết thúc";
                }
                return string.Empty;
            }
        }
    }
}
