using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ComplaintEvidence
    {
        public Guid Id { get; set; }
        public Guid ComplaintId { get; set; }
        public Complaint Complaint { get; set; } = null!;

        public string FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; } // mime or extension
        public string? Caption { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
