using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;
using Planify_BackEnd.Services.GoogleDrive;
using Planify_BackEnd.Services.Medias;
using Planify_BackEnd.Services.User;
using System.IdentityModel.Tokens.Jwt;

namespace Planify_BackEnd.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly GoogleDriveService _googleService;
        private readonly IMediumService _mediumService;
        public ProfilesController(IProfileService profileService, GoogleDriveService googleService, IMediumService mediumService)
        {
            _profileService = profileService;
            _googleService = googleService;
            _mediumService = mediumService;
        }
        [HttpGet("{userId}")]
        public IActionResult GetUserProfileById(Guid userId)
        {
            try
            {
                //var userId = User.Claims.FirstOrDefault(c=>c.Type== JwtRegisteredClaimNames.Sub)?.Value;
                //if (userId == null)
                //{
                //    return Unauthorized();
                //}
                var response = _profileService.getUserProfileById(userId);
                if (response == null||response.Id==null)
                {
                    return NotFound("Cannot found any user with id: " + userId);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        public IActionResult UpdateProfile(ProfileUpdateModel updateProfile)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (!userId.Equals(updateProfile.Id+""))
                {
                    return BadRequest("You can't update other people profile!");
                }
                var p = _profileService.getUserProfileById(updateProfile.Id);
                if (p== null || p.FirstName==null)
                {
                    return NotFound("Not found any user with id "+updateProfile.Id);
                }
                var response = _profileService.UpdateProfile(updateProfile);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{userId}/image")]
        [Authorize]
        public async Task<IActionResult> UpdateAvatar(Guid userId, IFormFile imageFile)
        {
            try
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                if (!id.Equals(userId+""))
                {
                    return BadRequest("UserId login not match with userId update!");
                }
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var contentType = imageFile.ContentType;
                var fileStream = imageFile.OpenReadStream();
                var fileName = imageFile.FileName;

                var fileUrl = await _googleService.UploadFileAsync(fileStream, fileName, contentType);

                if (string.IsNullOrEmpty(fileUrl))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload image.");
                }

                var newMedium = new Medium
                {
                    MediaUrl = fileUrl
                };
                var medium = await _mediumService.CreateMediaItemAsync(newMedium);
                if (medium == null) return BadRequest("Cannot save image url!");
                _profileService.UpdateAvatar(userId, medium.Id);

                return Ok(fileUrl);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
