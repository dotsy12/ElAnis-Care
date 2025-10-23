using ElAnis.Entities.DTO.ServiceProviderProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.Availability
{
    public class AvailabilityCalendarResponse
    {
        public List<AvailabilityDto> Availability { get; set; } = new();
        public List<ServiceRequestSummary> BookedDates { get; set; } = new();
    }
}
