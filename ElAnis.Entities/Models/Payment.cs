using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; } = null!;

        // تفاصيل الدفع
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } // كاش، فيزا، فودافون كاش، إلخ
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        // معلومات المعاملة
        public string? TransactionId { get; set; } // رقم المعاملة من payment gateway
        public string? PaymentGatewayResponse { get; set; } // JSON response

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}