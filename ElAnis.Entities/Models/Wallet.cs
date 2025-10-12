using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }

        // المالك (ممكن يكون عميل أو مقدم خدمة)
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        public WalletOwnerType OwnerType { get; set; } // Client أو Provider

        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // كل العمليات المالية الخاصة بالمحفظة
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
    public enum WalletOwnerType
    {
        Client,
        Provider
}

