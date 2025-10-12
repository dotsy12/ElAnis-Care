using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ComplaintParticipant
    {
        public Guid Id { get; set; }

        public Guid ComplaintId { get; set; }
        public Complaint Complaint { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public string Role { get; set; } = string.Empty; // e.g. "Complainant", "Respondent", "Observer"
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
