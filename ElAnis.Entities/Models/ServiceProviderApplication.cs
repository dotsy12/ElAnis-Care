using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
	public class ServiceProviderApplication
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string UserId { get; set; } = null!;
		public User User { get; set; } = null!;

		// Personal Information
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }

		// Professional Information
		public string Bio { get; set; } = string.Empty;
		public string NationalId { get; set; } = string.Empty;
		public string Experience { get; set; } = string.Empty;
		public decimal HourlyRate { get; set; }

		// Documents
		public string IdDocumentPath { get; set; } = string.Empty;
		public string CertificatePath { get; set; } = string.Empty;

		// Selected Categories
		public List<Guid> SelectedCategories { get; set; } = new();
		// JSON string of category IDs

		// Application Status
		public ServiceProviderApplicationStatus Status { get; set; } = ServiceProviderApplicationStatus.Pending;
		public string? RejectionReason { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? ReviewedAt { get; set; }
		public string? ReviewedById { get; set; }
		public User? ReviewedBy { get; set; }
	}
}
