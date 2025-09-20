using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
	public class Category
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty; // "جليسة أطفال", "تمريض منزلي"
		public string NameEn { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Icon { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation Properties
		public ICollection<ServiceProviderCategory> ServiceProviders { get; set; } = new List<ServiceProviderCategory>();
		public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
	}
}
