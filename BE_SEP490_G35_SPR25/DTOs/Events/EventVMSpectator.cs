using Planify_BackEnd.DTOs.Activity;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.DTOs.Medias;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Events
{
    public class EventVMSpectator
    {

        public int Id { get; set; }

        public string EventTitle { get; set; } = null!;

        public string? EventDescription { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? IsPublic { get; set; }

        public DateTime? TimePublic { get; set; }

        public int? Status { get; set; }

        public int? CampusId { get; set; }

        public int? CategoryEventId { get; set; }

        public string? Placed { get; set; } = null!;
        public bool? isFavorite { get; set; }
        public string? TargetAudience { get; set; }

        public string? SloganEvent { get; set; }

        public CampusDTO? CampusDTO { get; set; }

        public CategoryViewModel? CategoryViewModel { get; set; }
        public ICollection<ActivityDTO> Activities { get; set; }

        public ICollection<EventMediumViewMediaModel> EventMedias { get; set; } = new List<EventMediumViewMediaModel>();
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
