using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class Complaint
    {
        
            public Guid Id { get; set; }

            public string ComplaintNumber { get; set; } = string.Empty;

            // Optional: ربط بطلب خدمة
            public Guid? ServiceRequestId { get; set; }
            public ServiceRequest? ServiceRequest { get; set; }

            // المشتكي والمشتكى ضده (Users)
            public string ComplainantUserId { get; set; } = null!;
            public User ComplainantUser { get; set; } = null!;

            public string AgainstUserId { get; set; } = null!;
            public User AgainstUser { get; set; } = null!;

            // لو عايزين نصنف بالشكاوى ديناميكياً
            public Guid? CategoryId { get; set; }
            public ComplaintCategory? Category { get; set; }

            // تفاصيل الشكوى
            public ComplaintType ComplaintType { get; set; } = ComplaintType.Other;
            public string Subject { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;

            // الأولوية والحالة
            public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;
            public ComplaintStatus Status { get; set; } = ComplaintStatus.Submitted;

            // المعين من الإدارة لمتابعة الشكوى
            public string? AssignedToUserId { get; set; }
            public User? AssignedToUser { get; set; }

            // الحل
            public string? ResolutionNotes { get; set; }
            public ComplaintResolutionAction? ResolutionAction { get; set; }
            public DateTime? ResolvedAt { get; set; }

            // Soft delete
            public bool IsDeleted { get; set; } = false;
            public DateTime? DeletedAt { get; set; }

            // Timestamps
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }

            // Navigation collections
            public ICollection<ComplaintMessage> Messages { get; set; } = new List<ComplaintMessage>();
            public ICollection<ComplaintEvidence> Evidence { get; set; } = new List<ComplaintEvidence>();
            public ICollection<ComplaintActivity> Activities { get; set; } = new List<ComplaintActivity>();
            public ICollection<ComplaintParticipant> Participants { get; set; } = new List<ComplaintParticipant>();
      
    }

        // Enums (retain previous values)
        public enum ComplaintType
        {
            ServiceQuality,
            UnprofessionalBehavior,
            PaymentIssue,
            SafetyConcern,
            Other
        }

        public enum ComplaintPriority
        {
            Low,
            Medium,
            High,
            Urgent
        }

        public enum ComplaintStatus
        {
            Submitted,
            UnderReview,
            Investigating,
            Resolved,
            Closed,
            Escalated
        }

        public enum ComplaintResolutionAction
        {
            Warning,
            PartialRefund,
            FullRefund,
            AccountSuspension,
            NoAction
        }
    
}
