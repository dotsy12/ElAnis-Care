using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ElAnis.Entities.DTO.Upload;
using ElAnis.Utilities.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElAnis.DataAccess.Services.FileUpload
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        // ✅ Allowed file types
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private readonly string[] _allowedDocExtensions = { ".pdf", ".doc", ".docx" };
        private readonly string[] _allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv" };

        // ✅ Max file sizes (in bytes)
        private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB
        private const long MaxDocumentSize = 10 * 1024 * 1024; // 10 MB
        private const long MaxVideoSize = 50 * 1024 * 1024; // 50 MB

        public CloudinaryService(
            IOptions<CloudinarySettings> cloudinaryOptions,
            ILogger<CloudinaryService> logger)
        {
            var settings = cloudinaryOptions.Value;
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
            _logger = logger;
        }

        // ✅ Upload Image
        public async Task<UploadResultDto?> UploadImageAsync(IFormFile file, string? folder = null)
        {
            if (!ValidateImage(file))
                return null;

            try
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder ?? "elanis/images",
                    Transformation = new Transformation()
                        .Width(800)
                        .Height(800)
                        .Crop("limit")
                        .Quality("auto:good")
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", result.Error.Message);
                    return null;
                }

                return new UploadResultDto
                {
                    Url = result.SecureUrl.ToString(),
                    PublicId = result.PublicId,
                    Format = result.Format,
                    Size = result.Bytes,
                    ResourceType = "image"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary");
                return null;
            }
        }

        // ✅ Upload Document (PDF, DOCX)
        public async Task<UploadResultDto?> UploadDocumentAsync(IFormFile file, string? folder = null)
        {
            if (!ValidateDocument(file))
                return null;

            try
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder ?? "elanis/documents"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", result.Error.Message);
                    return null;
                }

                return new UploadResultDto
                {
                    Url = result.SecureUrl.ToString(),
                    PublicId = result.PublicId,
                    Format = result.Format,
                    Size = result.Bytes,
                    ResourceType = "raw"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document to Cloudinary");
                return null;
            }
        }

        // ✅ Upload Video
        public async Task<UploadResultDto?> UploadVideoAsync(IFormFile file, string? folder = null)
        {
            if (!ValidateVideo(file))
                return null;

            try
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder ?? "elanis/videos"
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    _logger.LogError("Cloudinary upload error: {Error}", result.Error.Message);
                    return null;
                }

                return new UploadResultDto
                {
                    Url = result.SecureUrl.ToString(),
                    PublicId = result.PublicId,
                    Format = result.Format,
                    Size = result.Bytes,
                    ResourceType = "video"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video to Cloudinary");
                return null;
            }
        }

        // ✅ Delete File
        public async Task<bool> DeleteFileAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return false;

            try
            {
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Cloudinary: {PublicId}", publicId);
                return false;
            }
        }

        // ✅ Validate Image
        public bool ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File is null or empty");
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid image extension: {Extension}", extension);
                return false;
            }

            if (file.Length > MaxImageSize)
            {
                _logger.LogWarning("Image size exceeds limit: {Size}", file.Length);
                return false;
            }

            return true;
        }

        // ✅ Validate Document
        public bool ValidateDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File is null or empty");
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedDocExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid document extension: {Extension}", extension);
                return false;
            }

            if (file.Length > MaxDocumentSize)
            {
                _logger.LogWarning("Document size exceeds limit: {Size}", file.Length);
                return false;
            }

            return true;
        }

        // ✅ Validate Video
        public bool ValidateVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File is null or empty");
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedVideoExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid video extension: {Extension}", extension);
                return false;
            }

            if (file.Length > MaxVideoSize)
            {
                _logger.LogWarning("Video size exceeds limit: {Size}", file.Length);
                return false;
            }

            return true;
        }
    }
}