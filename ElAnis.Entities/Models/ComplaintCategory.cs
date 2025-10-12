using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class ComplaintCategory
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty; // internal key, e.g. "payment_issue"
        public string Name { get; set; } = string.Empty; // display name
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
