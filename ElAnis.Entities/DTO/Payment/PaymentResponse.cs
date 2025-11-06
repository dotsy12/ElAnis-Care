using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.Payment
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        // Stripe Checkout URL
        public string? CheckoutUrl { get; set; }
    }
}