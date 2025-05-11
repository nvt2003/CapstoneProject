namespace Planify_BackEnd.DTOs.JoinedProjects
{
    public class AddImplementersToEventDTO
    {
        public int EventId { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
