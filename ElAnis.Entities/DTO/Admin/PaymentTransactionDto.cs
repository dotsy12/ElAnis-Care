namespace ElAnis.Entities.DTO.Admin
{
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty; // "completed", "pending", "failed"
        public string PaymentMethod { get; set; } = string.Empty;
        public Guid RequestId { get; set; }
    }

    public class PaymentSummaryResponse
    {
        public decimal TotalRevenue { get; set; }
        public List<PaymentTransactionDto> Transactions { get; set; } = new();
    }
}