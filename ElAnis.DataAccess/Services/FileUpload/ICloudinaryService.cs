using ElAnis.Entities.DTO.Upload;
using Microsoft.AspNetCore.Http;

namespace ElAnis.DataAccess.Services.FileUpload
{
    public interface ICloudinaryService
    {
        Task<UploadResultDto?> UploadImageAsync(IFormFile file, string? folder = null);
        Task<UploadResultDto?> UploadDocumentAsync(IFormFile file, string? folder = null);
        Task<UploadResultDto?> UploadVideoAsync(IFormFile file, string? folder = null);
        Task<bool> DeleteFileAsync(string publicId);
        bool ValidateImage(IFormFile file);
        bool ValidateDocument(IFormFile file);
        bool ValidateVideo(IFormFile file);
    }
}