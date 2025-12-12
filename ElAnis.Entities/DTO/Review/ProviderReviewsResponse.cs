namespace ElAnis.Entities.DTO.Review
{
    public class ProviderReviewsResponse
    {
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewResponse> Reviews { get; set; } = new();
    }
}