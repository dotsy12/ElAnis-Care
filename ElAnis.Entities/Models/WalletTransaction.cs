using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class WalletTransaction
    {
        public Guid Id { get; set; }

        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;

        public decimal Amount { get; set; }
        public WalletTransactionType Type { get; set; } = WalletTransactionType.Credit;

        public string? Description { get; set; }

        // لو المعاملة جاية من عملية دفع أو طلب خدمة
        public Guid? RelatedPaymentId { get; set; }
        public Payment? RelatedPayment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public enum WalletTransactionType
    {
        Credit,  // إضافة رصيد
        Debit    // خصم رصيد
    }
}

