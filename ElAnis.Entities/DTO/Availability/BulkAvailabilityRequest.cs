using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.Availability
{

    public class BulkAvailabilityRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAvailable { get; set; }
        public ShiftType? AvailableShift { get; set; }
        public List<DayOfWeek>? ExcludeDays { get; set; } // مثلاً استثني الجمعة والسبت
    }
}
