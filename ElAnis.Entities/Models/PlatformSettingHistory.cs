using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class PlatformSettingHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SettingId { get; set; }
        public string Key { get; set; } = null!;
        public string OldValue { get; set; } = null!;
        public string NewValue { get; set; } = null!;
        public string? ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }   // optional note from admin
    }
}
