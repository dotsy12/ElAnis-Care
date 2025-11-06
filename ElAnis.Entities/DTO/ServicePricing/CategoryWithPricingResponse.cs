using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.ServicePricing
{
    public class CategoryWithPricingResponse
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDescription { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public bool CategoryIsActive { get; set; }
        public List<ServicePricingResponse> Pricing { get; set; } = new();
    }
}
