using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    
        public class ServiceProviderCategory
        {
            public Guid ServiceProviderId { get; set; }
            public ServiceProviderProfile ServiceProvider { get; set; } = null!;

            public Guid CategoryId { get; set; }
            public Category Category { get; set; } = null!;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    
}
