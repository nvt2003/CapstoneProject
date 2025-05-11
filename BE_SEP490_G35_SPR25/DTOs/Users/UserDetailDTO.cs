namespace Planify_BackEnd.DTOs.Users
{
    public class UserDetailDTO
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Password { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; }

        public int? AvatarId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CampusName { get; set; }

        public int Status { get; set; }
        public string Gender { get; set; }
    }
}
