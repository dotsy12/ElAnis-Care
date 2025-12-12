using Microsoft.AspNetCore.Http;

namespace ElAnis.Entities.DTO.Upload
{
    public class UploadResultDto
    {
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ResourceType { get; set; } = string.Empty; // image, video, raw
    }

    public class FileUploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? Folder { get; set; } // Optional: organize by folder
    }
}