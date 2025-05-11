namespace Planify_BackEnd.DTOs.Users
{
    public class EventOrganizerUpdate
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int CampusId { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool Gender { get; set; }

        public string PhoneNumber { get; set; }
        public int? roleId { get; set; }
    }
}
