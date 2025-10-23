using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.ServiceProviderProfile
{
    // ===== Dashboard Response =====
    public class ProviderDashboardResponse
    {
        public Guid ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public bool IsAvailable { get; set; }
        public ServiceProviderStatus Status { get; set; }

        // Statistics
        public DashboardStatistics Statistics { get; set; } = new();

        // Recent Requests
        public List<ServiceRequestSummary> RecentRequests { get; set; } = new();

        // Upcoming Jobs
        public List<ServiceRequestSummary> UpcomingJobs { get; set; } = new();

        // Categories
        public List<CategorySummary> Categories { get; set; } = new();

        // Working Areas
        public List<string> WorkingAreas { get; set; } = new();
    }

    public class DashboardStatistics
    {
        public int CompletedJobs { get; set; }
        public int PendingRequests { get; set; }
        public int UpcomingJobs { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal CurrentMonthEarnings { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int WorkedDays { get; set; }
    }

    public class ServiceRequestSummary
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime PreferredDate { get; set; }
        public ShiftType ShiftType { get; set; }
        public string ShiftTypeName { get; set; } = string.Empty;
        public ServiceRequestStatus Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
    }

    public class CategorySummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

}
