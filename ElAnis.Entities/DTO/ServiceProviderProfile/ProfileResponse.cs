using ElAnis.Entities.DTO.WorkingArea;
using ElAnis.Utilities.Enum;


namespace ElAnis.Entities.DTO.ServiceProviderProfile
{
    public class ProviderProfileResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public string? Experience { get; set; }
        public string? NationalId { get; set; }
        public bool IsAvailable { get; set; }
        public ServiceProviderStatus Status { get; set; }

        // Statistics
        public int CompletedJobs { get; set; }
        public decimal TotalEarnings { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Related Data
        public List<CategorySummary> Categories { get; set; } = new();
        public List<WorkingAreaDto> WorkingAreas { get; set; } = new();
    }
}
