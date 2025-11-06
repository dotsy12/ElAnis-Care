using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.ServiceRequest
{
    public class ProviderResponseDto
    {
        public ServiceRequestStatus Status { get; set; }  // Approved or Rejected
        public string? Reason { get; set; }  // Required if Rejected
    }
}