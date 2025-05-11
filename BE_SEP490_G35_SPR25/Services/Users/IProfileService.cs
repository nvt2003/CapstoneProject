using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.User;
using Planify_BackEnd.DTOs.Users;

namespace Planify_BackEnd.Services.User
{
    public interface IProfileService
    {
        ProfileViewModel getUserProfileById(Guid id);
        ResponseDTO UpdateProfile(ProfileUpdateModel updateProfile);
        bool UpdateAvatar(Guid id, int avatarId);
    }
}
