using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;


namespace ElAnis.Entities.Models
{
	public class ServiceProviderProfile
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = null!;
		public User User { get; set; } = null!;

		// Personal Information
		public string? Bio { get; set; }
		public string? NationalId { get; set; }
		public string? Experience { get; set; }
		public decimal HourlyRate { get; set; }
		public string? IdDocumentPath { get; set; }
		public string? CertificatePath { get; set; }

		// Statistics for Dashboard
		public int CompletedJobs { get; set; } = 0;
		public decimal TotalEarnings { get; set; } = 0;
		public double AverageRating { get; set; } = 0;
		public int TotalReviews { get; set; } = 0;
		public int WorkedDays { get; set; } = 0;

		// Status & Availability
		public ServiceProviderStatus Status { get; set; } = ServiceProviderStatus.Pending;
		public bool IsAvailable { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? ApprovedAt { get; set; }
		public string? RejectionReason { get; set; }

		// Navigation Properties
		public ICollection<ServiceProviderCategory> Categories { get; set; } = new List<ServiceProviderCategory>();
		public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
		public ICollection<Review> Reviews { get; set; } = new List<Review>();
	}
}
