using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        // User الذي قام بالدفع (العميل)
        public string ClientUserId { get; set; } = null!;
        public User Client { get; set; } = null!;

        // المستخدم المستفيد من الدفع (مقدم الخدمة)
        public string? ProviderUserId { get; set; }
        public User? Provider { get; set; }


        // Optional: إذا الدفع مرتبط بخدمة محددة
        public Guid? ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }

        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.CreditCard;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        Wallet,
        Cash,
        BankTransfer
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}

