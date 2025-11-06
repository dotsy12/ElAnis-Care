using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.Provider
{
    public class ProviderDetailResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
        public List<ProviderWorkingAreaDto> WorkingAreas { get; set; } = new();
        public List<AvailabilityDto> Availability { get; set; } = new();
        public List<ShiftPriceDto> ShiftPrices { get; set; } = new();
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ProviderWorkingAreaDto
    {
        public string Governorate { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
    }

    public class AvailabilityDto
    {
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
        public ShiftType? AvailableShift { get; set; }
        public string? ShiftName { get; set; }
    }

    public class ShiftPriceDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ShiftType ShiftType { get; set; }
        public string ShiftTypeName { get; set; } = string.Empty;
        public decimal PricePerShift { get; set; }
        public Guid PricingId { get; set; }
    }
}