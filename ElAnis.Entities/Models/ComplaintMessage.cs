using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ComplaintMessage
    {
        public Guid Id { get; set; }

        public Guid ComplaintId { get; set; }
        public Complaint Complaint { get; set; } = null!;

        public string SenderUserId { get; set; } = null!;
        public User SenderUser { get; set; } = null!;

        public ComplaintMessageSenderType SenderType { get; set; } = ComplaintMessageSenderType.Client;

        public string Message { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }

        // Soft delete (optional)
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum ComplaintMessageSenderType
    {
        Client,
        Provider,
        Admin
    }
}
