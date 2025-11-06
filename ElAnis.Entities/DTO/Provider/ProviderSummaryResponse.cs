namespace ElAnis.Entities.DTO.Provider
{
    public class ProviderSummaryResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
        public LocationDto Location { get; set; } = new();
        public bool IsAvailable { get; set; }
        public double AverageRating { get; set; }
        public decimal HourlyRate { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LocationDto
    {
        public string Governorate { get; set; } = string.Empty;
        public string? City { get; set; }
    }
}