namespace Planify_BackEnd.DTOs.Categories
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string CategoryEventName { get; set; }

        public int? CampusId { get; set; }

        public int? Status { get; set; }
    }
}
