namespace ElAnis.Entities.Models
{
    public class ServiceCategory
    {
        public Guid Id { get; set; }

        public Guid ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;

        public string Name { get; set; } = string.Empty;               // اسم الفئة
        public string? Description { get; set; }                       // وصف الفئة
        public decimal? BasePrice { get; set; }                        // سعر مبدئي (اختياري)
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;                     // لترتيب الظهور في الواجهة
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}