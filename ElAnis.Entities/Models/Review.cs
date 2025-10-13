using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace ElAnis.Entities.Models
{
	public class Review
	{
        public Guid Id { get; set; }

        // الربط بطلب الخدمة
        public Guid ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; } = null!;

        // من الذي كتب التقييم؟
        public string ReviewerUserId { get; set; } = null!;
        public User ReviewerUser { get; set; } = null!;

        // من الذي تم تقييمه؟
        public string ReviewedUserId { get; set; } = null!;
        public User ReviewedUser { get; set; } = null!;

        public ReviewerType ReviewerType { get; set; }

        // تفاصيل التقييم
        public int Rating { get; set; }  // 1–5 نجوم
        public string? Comment { get; set; }
        public string? Tags { get; set; } // JSON array ["ملتزم", "محترف", ...]

        // المراجعة والتحقق
        public bool IsVerified { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public bool AdminHidden { get; set; } = false;
        public string? HiddenReason { get; set; }

        // الوقت
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}