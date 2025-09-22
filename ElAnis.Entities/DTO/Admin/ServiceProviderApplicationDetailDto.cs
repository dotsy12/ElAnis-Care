
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.Admin
{
	public class ServiceProviderApplicationDetailDto
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = string.Empty;
		public string UserEmail { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string Bio { get; set; } = string.Empty;
		public string NationalId { get; set; } = string.Empty;
		public string Experience { get; set; } = string.Empty;
		public decimal HourlyRate { get; set; }
		public string IdDocumentPath { get; set; } = string.Empty;
		public string CertificatePath { get; set; } = string.Empty;
		public List<Guid> SelectedCategories { get; set; } = new();

		public ServiceProviderApplicationStatus Status { get; set; }
		public string? RejectionReason { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ReviewedAt { get; set; }
		public string? ReviewedByName { get; set; }
	}
}