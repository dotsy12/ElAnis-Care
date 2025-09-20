using ElAnis.Entities.Models.Auth.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElAnis.Entities.Models
{
	public class Review
	{
		public Guid Id { get; set; }

		// User اللي عامل التقييم (العميل)
		public string ClientUserId { get; set; } = null!;
		public User Client { get; set; } = null!;

		// User اللي باخد التقييم (مقدم الخدمة)
		public string ServiceProviderUserId { get; set; } = null!;
		public User ServiceProvider { get; set; } = null!;
		public Guid ServiceRequestId { get; set; } // بدل int
		public ServiceRequest ServiceRequest { get; set; } = null!;

		[Range(1, 5)]
		public int Rating { get; set; } // 1-5

		[MaxLength(1000)]
		public string? Comment { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}