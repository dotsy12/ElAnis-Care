namespace ElAnis.Entities.DTO.Review
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientAvatar { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}