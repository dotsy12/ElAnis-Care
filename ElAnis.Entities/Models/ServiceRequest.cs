using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;


namespace ElAnis.Entities.Models
{
	public class ServiceRequest
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = null!;
		public User User { get; set; } = null!;

		public Guid ? ServiceProviderId { get; set; }
		public ServiceProviderProfile? ServiceProvider { get; set; }

		public Guid CategoryId { get; set; }
		public Category Category { get; set; } = null!;

		public string Description { get; set; } = string.Empty;

        public string Governorate { get; set; } = string.Empty; // المحافظة



        public string Address { get; set; } = string.Empty;
		public DateTime PreferredDate { get; set; }


        // السعر (يتم حسابه من ServicePricing)
        public decimal TotalPrice { get; set; }

     
        public ShiftType ShiftType { get; set; } // نوع الشيفت (3ساعات، 12ساعة، 24ساعة)
        public decimal? OfferedPrice { get; set; }

		public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; } // وقت قبول البروفايدر
        public DateTime? StartedAt { get; set; } // وقت بدء الخدمة
        public DateTime? CompletedAt { get; set; } // وقت انتهاء الخدمة

        // Navigation Properties
        public Payment? Payment { get; set; }
        public Review? Review { get; set; }
    }
}
