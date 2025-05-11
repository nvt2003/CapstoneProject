namespace Planify_BackEnd.DTOs.Implementers
{
    public class JoinGroupDTO
    {
        public int Id { get; set; }

        public Guid ImplementerId { get; set; }

        public int GroupId { get; set; }

        public DateTime TimeJoin { get; set; }

        public DateTime? TimeOut { get; set; }

        public int Status { get; set; }
    }
}
