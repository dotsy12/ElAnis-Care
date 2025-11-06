using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.ServicePricing
{
    /// <summary>
    /// لإضافة أسعار متعددة لنفس الـ Category مرة واحدة
    /// </summary>
    public class BulkServicePricingRequest
    {
        public Guid CategoryId { get; set; }
        public List<PricingItem> Pricings { get; set; } = new();
    }

    public class PricingItem
    {
        public ShiftType ShiftType { get; set; }
        public decimal PricePerShift { get; set; }
        public string? Description { get; set; }
    }
}
