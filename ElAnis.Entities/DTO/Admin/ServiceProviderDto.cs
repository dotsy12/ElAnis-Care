
// ServiceProviderDto.cs
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.Admin
{
	public class ServiceProviderDto
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = string.Empty;
		public string UserEmail { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public decimal HourlyRate { get; set; }
		public ServiceProviderStatus Status { get; set; }
		public bool IsAvailable { get; set; }
		public int CompletedJobs { get; set; }
		public decimal TotalEarnings { get; set; }
		public double AverageRating { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}