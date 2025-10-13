using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class AdminAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AdminUserId { get; set; } = null!;      // FK to User.Id (string if Identity)
        public AdminActionType ActionType { get; set; }       // enum or lookup
        public string TargetType { get; set; } = null!;       // e.g., "User","Request","Payment","Complaint"
        public Guid TargetId { get; set; }                    // polymorphic target identifier
        public string? ActionDetails { get; set; }            // JSON payload or note
        public string? IpAddress { get; set; }
        public string? DeviceInfo { get; set; }               // optional: user-agent/device id
        public string? CorrelationId { get; set; }            // trace requests across services
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User AdminUser { get; set; } = null!;
    }
}
