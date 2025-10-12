using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ServiceExecution
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }    // الخدمة المرتبطة
        public Guid ProviderId { get; set; }   // مقدم الخدمة

        public ExecutionStatus Status { get; set; } = ExecutionStatus.Scheduled;

        public DateTime? CheckInTime { get; set; }
        public string? CheckInLocation { get; set; }

        public DateTime? CheckOutTime { get; set; }
        public string? CheckOutLocation { get; set; }

        public int? ActualDuration { get; set; } // بالدقائق
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // 🔗 Navigation
        public User Provider { get; set; } = null!;
        public ICollection<ServiceReport> Reports { get; set; } = new List<ServiceReport>();
        public ICollection<ServiceTracking> Trackings { get; set; } = new List<ServiceTracking>();
    }

    public enum ExecutionStatus
    {
        Scheduled,
        EnRoute,
        Started,
        Paused,
        Resumed,
        Completed,
        Cancelled
    }
}
