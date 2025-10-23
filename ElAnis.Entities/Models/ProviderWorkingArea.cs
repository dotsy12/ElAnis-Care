namespace ElAnis.Entities.Models
{
    public class ProviderWorkingArea
    {
        public Guid Id { get; set; }
        public Guid ServiceProviderId { get; set; }
        public ServiceProviderProfile ServiceProvider { get; set; } = null!;

        // المنطقة الجغرافية
        public string Governorate { get; set; } = string.Empty; // القاهرة، الجيزة، الإسكندرية
        public string? City { get; set; } // المدينة داخل المحافظة
        public string? District { get; set; } // الحي

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}