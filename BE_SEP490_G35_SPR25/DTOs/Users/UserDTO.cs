namespace Planify_BackEnd.DTOs.Users
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int CampusId { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool Gender { get; set; }

        public string PhoneNumber { get; set; }

    }
}
