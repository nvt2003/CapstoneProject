using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Andress
{
    public class DistrictVM
    {
        public int Id { get; set; }

        public string DistrictName { get; set; }

        public ProvinceVM ProvinceVM { get; set; }
    }
}
