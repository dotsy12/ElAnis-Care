using Microsoft.AspNetCore.Http;

namespace ElAnis.Entities.DTO.User
{
    public class UpdateProfilePictureRequest
    {
        public IFormFile ProfilePicture { get; set; } = null!;
    }
}