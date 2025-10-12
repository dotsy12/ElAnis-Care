using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ServiceTracking
    {
        public Guid Id { get; set; }

        public Guid ServiceExecutionId { get; set; }
        public ServiceExecution ServiceExecution { get; set; } = null!;

        public Guid ProviderId { get; set; }
        public User Provider { get; set; } = null!;

        public decimal Latitude { get; set; }  // 9,6 كفاية للدقة
        public decimal Longitude { get; set; }

        public TrackingStatus TrackingStatus { get; set; } = TrackingStatus.HeadingToLocation;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }

    public enum TrackingStatus
    {
        HeadingToLocation,
        AtLocation,
        ServiceInProgress
    }
}
