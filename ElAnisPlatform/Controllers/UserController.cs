using ElAnis.DataAccess;
using ElAnis.DataAccess.Services.FileUpload;
using ElAnis.Entities.DTO.User;
using ElAnis.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElAnis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ResponseHandler _responseHandler;

        public UserController(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            ResponseHandler responseHandler)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _responseHandler = responseHandler;
        }

        /// <summary>
        /// Update user profile picture
        /// </summary>
        [HttpPut("profile-picture")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(_responseHandler.Unauthorized<string>("User not authenticated"));

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return NotFound(_responseHandler.NotFound<string>("User not found"));

                // ✅ Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicturePublicId))
                {
                    await _cloudinaryService.DeleteFileAsync(user.ProfilePicturePublicId);
                }

                // ✅ Upload new profile picture
                var uploadResult = await _cloudinaryService.UploadImageAsync(
                    request.ProfilePicture,
                    "elanis/users"
                );

                if (uploadResult == null)
                    return BadRequest(_responseHandler.BadRequest<string>("Failed to upload profile picture"));

                // ✅ Update user
                user.ProfilePicture = uploadResult.Url;
                user.ProfilePicturePublicId = uploadResult.PublicId;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return Ok(_responseHandler.Success(uploadResult.Url, "Profile picture updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, _responseHandler.ServerError<string>("Error updating profile picture"));
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(Response<object>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(_responseHandler.Unauthorized<object>("User not authenticated"));

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return NotFound(_responseHandler.NotFound<object>("User not found"));

                var profile = new
                {
                    user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    user.Email,
                    user.PhoneNumber,
                    user.ProfilePicture,
                    user.Address,
                    user.CreatedAt
                };

                return Ok(_responseHandler.Success<object>(profile, "Profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, _responseHandler.ServerError<object>("Error retrieving profile"));
            }
        }
    }
}