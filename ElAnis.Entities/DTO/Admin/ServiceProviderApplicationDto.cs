using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


	namespace ElAnis.Entities.DTO.Admin
	{
		public class ServiceProviderApplicationDto
		{
			public Guid Id { get; set; }
			public string UserId { get; set; } = string.Empty;
			public string UserEmail { get; set; } = string.Empty;
			public string FirstName { get; set; } = string.Empty;
			public string LastName { get; set; } = string.Empty;
			public string PhoneNumber { get; set; } = string.Empty;
			public string Bio { get; set; } = string.Empty;
			public string Experience { get; set; } = string.Empty;
			public decimal HourlyRate { get; set; }
			public ServiceProviderApplicationStatus Status { get; set; }
			public DateTime CreatedAt { get; set; }
			public DateTime? ReviewedAt { get; set; }
			public string? ReviewedByName { get; set; }
			public string? RejectionReason { get; set; }
		}
	}


