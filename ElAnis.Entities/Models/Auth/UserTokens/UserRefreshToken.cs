using System.ComponentModel.DataAnnotations;

using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.Entities.Models.Auth.UserTokens
{
    public class UserRefreshToken
    {
		public Guid Id { get; set; }
		public string UserId { get; set; } = null!;
		public string Token { get; set; } = null!;
		public DateTime ExpiryDateUtc { get; set; }
		public bool IsUsed { get; set; } = false;
		public bool IsRevoked { get; set; } = false;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? CreatedByIp { get; set; }
		public DateTime? RevokedAt { get; set; }
		public string? RevokedByIp { get; set; }
	}
}
