using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.FavouriteEvents
{
    public class FavouriteEventGetDTO
    {
        public int Id { get; set; }

        public int? EventId { get; set; }

        public Guid? UserId { get; set; }
    

        
    }
}
