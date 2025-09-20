using Microsoft.AspNetCore.Http;

namespace ElAnis.DataAccess.Services.ImageUploading
{
    public interface IImageUploadService
    {
        Task<string> UploadAsync(IFormFile file);

    }
}
