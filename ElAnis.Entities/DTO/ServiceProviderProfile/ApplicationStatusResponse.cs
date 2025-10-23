using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.ServiceProviderProfile
{
    public class ApplicationStatusResponse
    {
        public Guid ApplicationId { get; set; }
        public ServiceProviderApplicationStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public bool CanReapply { get; set; }

        // ✅ إضافة معلومات مهمة
        public bool HasProfile { get; set; } // هل تم إنشاء Profile؟
        public bool NeedsTokenRefresh { get; set; } // هل يحتاج Refresh للـ Token؟

        // ✅ للحالة Approved
        public string? NextAction { get; set; } // مثلاً: "Refresh your token to access dashboard"
        public string? RefreshTokenEndpoint { get; set; } // "/api/auth/refresh-token"
    }
}