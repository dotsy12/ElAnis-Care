using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


	namespace ElAnis.Entities.DTO.Admin
	{
		public class AdminDashboardStatsDto
		{
			public int TotalUsers { get; set; }
			public int TotalServiceProviders { get; set; }
			public int PendingApplications { get; set; }
			public int TotalServiceRequests { get; set; }
			public int CompletedServiceRequests { get; set; }
			public int TotalReviews { get; set; }
			public decimal TotalEarnings { get; set; }
			public double AverageRating { get; set; }
		}
	}

