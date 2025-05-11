using Planify_BackEnd.DTOs.Andress;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.Users
{
    public class ProfileUpdateModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } 

        public string LastName { get; set; } 

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public int? AddressId { get; set; }

        public int? AvatarId { get; set; }

        public bool Gender { get; set; }
        public AddressVM addressVM { get; set; }
    }
}
