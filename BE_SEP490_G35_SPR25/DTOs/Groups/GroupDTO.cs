using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Groups
{
    public class GroupDTO
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public Guid CreateBy { get; set; }

        public int EventId { get; set; }

        public decimal AmountBudget { get; set; }
    }
}
