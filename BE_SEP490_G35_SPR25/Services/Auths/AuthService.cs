using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Planify_BackEnd.Models;
using Planify_BackEnd.Entities;
using Google.Apis.Auth;
using Planify_BackEnd.DTOs.Login;
using Planify_BackEnd.Services.Auths;
using Planify_BackEnd.DTOs;
using Newtonsoft.Json.Linq;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<ResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO request)
    {
        var googleUserInfo = await ValidateGoogleTokenAsync(request.GoogleToken);
        if (googleUserInfo == null || string.IsNullOrEmpty(googleUserInfo.Email))
        {
            return new ResponseDTO(400, "Invalid Google token or email not provided.", null);
        }

        var user = await _userRepository.GetUserByEmailAsync(googleUserInfo.Email);
        if (user == null || !string.Equals(request.CampusName, user.Campus.CampusName, StringComparison.OrdinalIgnoreCase))
        {
            return new ResponseDTO(401, "Email not found in the system. Please register or contact an admin.", null);
        }

        if (user.Status == 0)
        {
            return new ResponseDTO(401, "Your account has been banned.", null);
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new ResponseDTO(200, "Login successful!", new AuthResponseDTO
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Campus = user.Campus.CampusName,
            Role = user.UserRoles?.FirstOrDefault()?.Role?.RoleName,
            AccessToken = jwtToken,
            RefreshToken = refreshToken
        });
    }

    private async Task<GoogleUserInfo> ValidateGoogleTokenAsync(string googleToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            return new GoogleUserInfo
            {
                Email = payload.Email,
                Name = payload.Name
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Google Token Validation Failed: {ex.Message}");
            return null;
        }
    }



    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new Exception("JWT Key is not configured.");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expirationMinutes = _configuration["Jwt:AccessTokenExpirationMinutes"];

        if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(expirationMinutes))
        {
            throw new Exception("JWT configuration is incomplete.");
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roleName = user.UserRoles?.FirstOrDefault()?.Role?.RoleName;
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, roleName),
        new Claim("campusId", user.CampusId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(expirationMinutes)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<AuthResponseDTO> RefreshToken(string refreshToken, string accessToken)
    {
        // Giải mã refreshToken
        string decodedRefreshToken = System.Web.HttpUtility.UrlDecode(refreshToken);

        if (string.IsNullOrEmpty(decodedRefreshToken))
        {
            throw new Exception("Invalid refresh token.");
        }
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Invalid access token.");
        }

        JwtSecurityToken token;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            token = handler.ReadJwtToken(accessToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to read access token: {ex.Message}");
        }

        var expClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
        if (string.IsNullOrEmpty(expClaim))
        {
            throw new Exception("Access token does not contain expiration claim.");
        }

        if (!long.TryParse(expClaim, out long expUnixTime))
        {
            throw new Exception("Invalid expiration claim format.");
        }
        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).DateTime;

        if (expiryDate > DateTime.Now)
        {
            throw new Exception("Access token is still valid.");
        }

        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new Exception("Invalid user ID in access token.");
        }

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var newJwtToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        return new AuthResponseDTO
        {        
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Campus = user.Campus.CampusName,
            Role = user.UserRoles?.FirstOrDefault()?.Role?.RoleName,
            AccessToken = newJwtToken,
            RefreshToken = newRefreshToken
        };
    }
    public async Task<ResponseDTO> AdminLoginAsync(LoginRequestDTO request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return new ResponseDTO(400, "Username and password are required.", null);
        }

        var user = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (user == null)
        {
            return new ResponseDTO(401, "Invalid username or password.", null);
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return new ResponseDTO(401, "Invalid username or password.", null);
        }

        var isAdmin = user.UserRoles?.Any(ur => ur.RoleId == 1) ?? false;
        if (!isAdmin)
        {
            return new ResponseDTO(403, "Only Admin users can log in with username and password.", null);
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        return new ResponseDTO(200, "Admin login successful!", new AuthResponseDTO
        {
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Role = user.UserRoles?.FirstOrDefault(ur => ur.RoleId == 1)?.Role?.RoleName,
            AccessToken = jwtToken,
            RefreshToken = refreshToken
        });
    }
}