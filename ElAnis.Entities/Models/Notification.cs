// Notification.cs - للإشعارات
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;
using ElAnis.Utilities.Enum.ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
	public class Notification
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = null!;
		public User User { get; set; } = null!;

		public string Title { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public NotificationType Type { get; set; }
		public bool IsRead { get; set; } = false;

		// Optional: Link to specific entities
		// Notification
		public Guid? ServiceRequestId { get; set; }  // بدل int?
		public ServiceRequest? ServiceRequest { get; set; }  // خلي nullable لو FK اختياري

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}