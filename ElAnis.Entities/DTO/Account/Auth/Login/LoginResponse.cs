using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.Account.Auth.Login
{
   
		public class LoginResponse
		{
			public string Id { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string PhoneNumber { get; set; } = string.Empty;
			public string Role { get; set; } = string.Empty;
			public bool IsEmailConfirmed { get; set; }
			public ServiceProviderStatus? ProviderStatus { get; set; }
			public bool? IsAvailable { get; set; } // للـ Service Provider
			public string AccessToken { get; set; } = string.Empty;
			public string RefreshToken { get; set; } = string.Empty;
		}
}
