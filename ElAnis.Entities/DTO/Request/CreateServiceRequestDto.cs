using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.DTO.ServiceRequest
{
    public class CreateServiceRequestDto
    {
        public Guid? ProviderId { get; set; }  // Optional - null for open request
        public Guid CategoryId { get; set; }
        public ShiftType ShiftType { get; set; }
        public DateTime PreferredDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}