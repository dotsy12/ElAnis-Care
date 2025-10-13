using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class PriceRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ServiceTypeId { get; set; }          // FK
        public Guid? ServiceCategoryId { get; set; }     // optional
        public DurationType DurationType { get; set; }   // enum (hourly/daily/monthly)
        public decimal MinPrice { get; set; }
        public decimal RecommendedPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal PlatformCommissionPercent { get; set; } = 10.0m;
        public bool IsActive { get; set; } = true;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Currency { get; set; } = "EGP";

        // extensible: ranges/tiered pricing
        public ICollection<PriceRuleTier> Tiers { get; set; } = new List<PriceRuleTier>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
