// ===== 3. ServicePricing.cs (Updated) =====
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
    public class ServicePricing
    {
        public Guid Id { get; set; }

        // ربط بالـ Category
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // نوع الشيفت
        public ShiftType ShiftType { get; set; }

        // السعر
        public decimal PricePerShift { get; set; }

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // للتتبع: مين اللي عمل آخر تعديل
        public string? UpdatedBy { get; set; }
    }
}