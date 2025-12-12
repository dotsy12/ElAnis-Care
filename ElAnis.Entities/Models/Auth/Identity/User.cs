using Microsoft.AspNetCore.Identity;

namespace ElAnis.Entities.Models.Auth.Identity
{
	public class User : IdentityUser
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;

       // public string? Governorate { get; set; } // المحافظة
        public string? Address { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? ProfilePicture { get; set; }

        public string? ProfilePicturePublicId { get; set; } // ✅ أضف هذا

        // Account Status
        public bool IsActive { get; set; } = true;
		public bool IsDeleted { get; set; } = false;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? LastLoginDate { get; set; }

        // Navigation Properties
        public ServiceProviderProfile? ServiceProviderProfile { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
		public ICollection<Review> GivenReviews { get; set; } = new List<Review>();
		public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
		// علاقة One-to-one مع UserPreferences
	


		// علاقة One-to-One مع ServiceProviderApplication
		public ServiceProviderApplication ServiceProviderApplication { get; set; }

		// علاقة One-to-Many مع Notifications
		public ICollection<Notification> Notifications { get; set; }
	}
}
