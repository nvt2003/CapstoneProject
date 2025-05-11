using Planify_BackEnd.DTOs.Roles;

namespace Planify_BackEnd.DTOs.Users
{
    public class UserRoleDTO
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int RoleId { get; set; }
        public RoleDTO? RoleDTO { get; set; }
    }
}
