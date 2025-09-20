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
		public string Address { get; set; } = string.Empty;
		public DateTime PreferredDate { get; set; }
		public TimeSpan PreferredTime { get; set; }
		public decimal? OfferedPrice { get; set; }

		public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
