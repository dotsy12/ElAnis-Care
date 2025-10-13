using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class PlatformSetting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = null!;          // unique key, e.g., "commission.percent"
        public string Value { get; set; } = null!;        // current value as string (or JSON)
        public SettingType Type { get; set; }             // enum: Text, Number, Boolean, Json
        public string? Description { get; set; }
        public string? UpdatedByUserId { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
