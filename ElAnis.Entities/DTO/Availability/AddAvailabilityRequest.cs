using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.Availability
{

    public class AddAvailabilityRequest
    {
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
        public ShiftType? AvailableShift { get; set; }
        public string? Notes { get; set; }
    }
}
