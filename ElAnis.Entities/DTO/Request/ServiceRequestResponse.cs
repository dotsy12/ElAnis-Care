using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.ServiceRequest
{
    public class ServiceRequestResponse
    {
        public Guid Id { get; set; }
        public Guid? ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string? ProviderAvatar { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ServiceRequestStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime PreferredDate { get; set; }
        public ShiftType ShiftType { get; set; }
        public string ShiftTypeName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public bool CanPay { get; set; }  // Status == Accepted && Payment == null
    }
}