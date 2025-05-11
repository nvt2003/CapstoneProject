using Planify_BackEnd.DTOs.Medias;

namespace Planify_BackEnd.DTOs.Users
{
    public class EventOrganizerVM
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public bool Gender { get; set; }

        public MediumDTO? Avatar { get; set; }
        public string? CampusName{ get; set; }
    }
}
