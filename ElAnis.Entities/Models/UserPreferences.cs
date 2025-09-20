
using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.Entities.Models
{
	public class UserPreferences
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = null!;
		public User User { get; set; } = null!;

		// Notification preferences
		public bool EmailNotifications { get; set; } = true;
		public bool PushNotifications { get; set; } = true;
		public bool SmsNotifications { get; set; } = false;

		// App preferences
		public string PreferredLanguage { get; set; } = "ar";
		public string PreferredCurrency { get; set; } = "EGP";

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}