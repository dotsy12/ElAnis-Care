using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ComplaintActivity
    {
        public Guid Id { get; set; }

        public Guid ComplaintId { get; set; }
        public Complaint Complaint { get; set; } = null!;

        public string PerformedByUserId { get; set; } = null!;
        public User PerformedByUser { get; set; } = null!;

        public string ActionType { get; set; } = string.Empty; // e.g. "StatusChanged", "Assigned", "NoteAdded"
        public string? Description { get; set; } // human friendly note
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
