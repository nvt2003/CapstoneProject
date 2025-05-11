using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Medias
{
    public class EventMediumViewMediaModel
    {
        public int Id { get; set; }

        public int? EventId { get; set; }

        public int? MediaId { get; set; }

        public int? Status { get; set; }
        public MediaItemDTO? MediaDTO { get; set; }
    }
}
