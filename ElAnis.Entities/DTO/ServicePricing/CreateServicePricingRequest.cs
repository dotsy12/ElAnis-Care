using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.ServicePricing
{
    public class CreateServicePricingRequest
    {
        public Guid CategoryId { get; set; }
        public ShiftType ShiftType { get; set; }
        public decimal PricePerShift { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
