using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
    public class ProviderAvailability
    {
        public Guid Id { get; set; }
        public Guid ServiceProviderId { get; set; }
        public ServiceProviderProfile ServiceProvider { get; set; } = null!;

        // التاريخ والوقت
        public DateTime Date { get; set; } // التاريخ
        public bool IsAvailable { get; set; } = true; // متاح أم لا
        public ShiftType? AvailableShift { get; set; } // الشيفت المتاح (إذا كان متاح)

        // ملاحظات
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}