using System.ComponentModel.DataAnnotations;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Groups
{
    public class GroupCreateRequestDTO
    {
        [Required]
        public string? GroupName { get; set; }

        public Guid Organizer { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        public decimal AmountBudget { get; set; }
    }
}
