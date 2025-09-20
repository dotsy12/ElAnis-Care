using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.Account.Auth.Register
{
	public class RegisterServiceProviderRequest
	{
		// Basic Account Info
		public string Email { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string ConfirmPassword { get; set; } = string.Empty;

		// Personal Information
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }

		// Professional Information
		public string Bio { get; set; } = string.Empty;
		public string NationalId { get; set; } = string.Empty;
		public string Experience { get; set; } = string.Empty;
		public decimal HourlyRate { get; set; }

		// Documents (IFormFile for file uploads)
		public IFormFile? IdDocument { get; set; }
		public IFormFile? Certificate { get; set; }

		// Selected Categories (comma-separated IDs or JSON)
		public string SelectedCategoryIds { get; set; } = string.Empty;
	
	}
}
