using Planify_BackEnd.DTOs.Medias;

namespace Planify_BackEnd.DTOs.Users
{
    public class UserListDTO
    {
        public Guid Id { get; set; }

        public string? UserName { get; set; }

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Password { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public int? AddressId { get; set; }

        public int? AvatarId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int CampusId { get; set; }

        public int Status { get; set; }
        public bool Gender { get; set; }
        public MediaItemDTO? Avatar { get; set; }
        public string? RoleName {  get; set; }
    }
}
