using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.WorkingArea
{
    public class AddWorkingAreaRequest
    {
        public string Governorate { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
    }

}
