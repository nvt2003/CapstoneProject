using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs.User;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.User
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly PlanifyContext _context;
        public ProfileRepository(PlanifyContext context)
        {
            _context = context;
        }
        public Models.User GetUserProfileById(Guid id)
        {
            return _context.Users
                .Include(u=>u.Avatar)
                .Include(u=>u.Campus)
                .Include(u=>u.Address).ThenInclude(a=>a.Ward).ThenInclude(w=>w.District).ThenInclude(d=>d.Province)
                .Include(u=>u.UserRoles).ThenInclude(ur=>ur.Role)
                .FirstOrDefault(u => u.Id.Equals(id));
        }

        public bool UpdateAvatar(Guid id, int avatarId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id==id);
                user.AvatarId = avatarId;
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }catch
            {
                return false;
            }
        }

        public Models.User UpdateProfile(ProfileUpdateModel updateProfile)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == updateProfile.Id);
                user.FirstName = updateProfile.FirstName;
                user.LastName = updateProfile.LastName;
                user.DateOfBirth = updateProfile.DateOfBirth;
                user.Gender = updateProfile.Gender;
                user.AddressId = updateProfile.AddressId;
                user.PhoneNumber = updateProfile.PhoneNumber;
                _context.Update(user);
                _context.SaveChanges();
                return user;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
