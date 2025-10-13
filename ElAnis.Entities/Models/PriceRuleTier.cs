using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class PriceRuleTier
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PriceRuleId { get; set; }
        public decimal FromValue { get; set; }    // e.g., duration from 1 hour
        public decimal ToValue { get; set; }      // to 4 hours
        public decimal Price { get; set; }        // price for this tier
    }
}
