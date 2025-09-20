using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.Account.Auth.Register
{
	public class ServiceProviderApplicationResponse
	{
		public Guid ApplicationId { get; set; }
		public string UserId { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public ServiceProviderApplicationStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
	}

}
