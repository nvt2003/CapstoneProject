using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Andress
{
    public class AddressVM
    {
        public int Id { get; set; }

        public string AddressDetail { get; set; }

        public WardVM WardVM { get; set; }
    }
}
