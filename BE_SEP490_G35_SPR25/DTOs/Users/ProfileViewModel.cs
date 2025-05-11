using Planify_BackEnd.DTOs.Andress;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.DTOs.Medias;
using Planify_BackEnd.DTOs.Roles;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.DTOs.User
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

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

        public AddressVM? AddressVM { get; set; }

        public MediumDTO? Avatar { get; set; }

        public CampusDTO? CampusDTO { get; set; }
        public ICollection<UserRoleDTO> UserRoleDTO { get; set; } = new List<UserRoleDTO>();
    }
}
