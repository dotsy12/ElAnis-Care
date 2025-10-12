using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ServiceReport
    {
        public Guid Id { get; set; }

        public Guid ServiceExecutionId { get; set; }
        public ServiceExecution ServiceExecution { get; set; } = null!;

        public Guid ProviderId { get; set; }
        public User Provider { get; set; } = null!;

        public ReportType ReportType { get; set; }
        public DateTime ReportDate { get; set; }

        public string ReportContent { get; set; } = string.Empty;
        public string? Attachments { get; set; } // JSON array of image URLs

        public bool ClientAcknowledged { get; set; } = false;
        public DateTime? AcknowledgedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }

    public enum ReportType
    {
        Daily,
        Weekly,
        Final
    }
}
