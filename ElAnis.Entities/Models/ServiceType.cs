using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ServiceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;               // اسم نوع الخدمة
        public string? Description { get; set; }                       // وصف الخدمة
        public string? IconUrl { get; set; }                           // أيقونة للعرض في التطبيق
        public bool IsActive { get; set; } = true;                     // الحالة (نشط/متوقف)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public ICollection<ServiceCategory> Categories { get; set; } = new List<ServiceCategory>();
    }
}
