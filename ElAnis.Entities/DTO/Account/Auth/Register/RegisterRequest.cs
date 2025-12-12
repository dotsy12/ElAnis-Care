using Microsoft.AspNetCore.Http;

namespace ElAnis.Entities.DTO.Account.Auth.Register
{
    public class RegisterRequest
    {
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string? Address { get; set; }
        public IFormFile? ProfilePicture { get; set; }

        public DateTime? DateOfBirth { get; set; }
	}
}
