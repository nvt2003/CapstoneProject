using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Andress
{
    public class WardVM
    {
        public int Id { get; set; }

        public string WardName { get; set; }

        public DistrictVM DistrictVM { get; set; }
    }
}
