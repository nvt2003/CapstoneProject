using Planify_BackEnd.DTOs.Users;

namespace Planify_BackEnd.DTOs.Implementers
{
    public class JoinGroupVM
    {
        public int Id { get; set; }

        public Guid ImplementerId { get; set; }

        public DateTime TimeJoin { get; set; }

        public DateTime? TimeOut { get; set; }

        public int Status { get; set; }

        public UserNameVM Implementer { get; set; }
    }
}
