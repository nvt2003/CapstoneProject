namespace Planify_BackEnd.DTOs.JoinGroups
{
    public class JoinGroupRequestDTO
    {
        public List<Guid> ImplementerIds { get; set; }
        public int GroupId { get; set; }
    }

}
