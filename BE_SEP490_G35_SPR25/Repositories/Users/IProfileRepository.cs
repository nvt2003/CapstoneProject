

using Planify_BackEnd.DTOs.Users;

namespace Planify_BackEnd.Repositories.User
{
    public interface IProfileRepository
    {
        Models.User GetUserProfileById(Guid id);
        Models.User UpdateProfile(ProfileUpdateModel updateProfile);
        bool UpdateAvatar(Guid id, int avatarId);
    }
}
