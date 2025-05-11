namespace Planify_BackEnd.DTOs.Events
{
    public class ParticipantCountDTO
    {
        public int EventId { get; set; }
        public int TotalParticipants { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<ParticipantDetailDTO> Participants { get; set; } = new List<ParticipantDetailDTO>();
    }

    public class ParticipantDetailDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationTime { get; set; }
    }

    public class RegisterEventDTO
    {
        public int EventId { get; set; }
        public Guid UserId { get; set; }
    }

    public class RegisteredEventDTO
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime RegistrationTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Placed { get; set; }
        public bool isFavorite { get; set; }
        public ICollection<EventMediaDto> EventMedia { get; set; }
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
