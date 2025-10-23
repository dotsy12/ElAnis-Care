using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.WorkingArea
{

    public class UpdateWorkingAreaRequest
    {
        public Guid Id { get; set; }
        public string Governorate { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? District { get; set; }
        public bool IsActive { get; set; }
    }
}
