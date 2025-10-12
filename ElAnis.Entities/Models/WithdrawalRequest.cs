using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class WithdrawalRequest
    {
        public Guid Id { get; set; }

        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;

        public decimal Amount { get; set; }
        public WithdrawalRequestStatus Status { get; set; } = WithdrawalRequestStatus.Pending;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; } // Id of admin or manager
    }
    public enum WithdrawalRequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Completed
    }
}
