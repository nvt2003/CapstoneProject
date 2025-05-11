using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Login;
using Planify_BackEnd.Entities;

namespace Planify_BackEnd.Services.Auths
{
    public interface IAuthService
    {
        Task<ResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO request);
        Task<AuthResponseDTO> RefreshToken(string refreshToken, string accessToken);
        Task<ResponseDTO> AdminLoginAsync(LoginRequestDTO request);
    }
}
