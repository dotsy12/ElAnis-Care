using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.DTO.ServiceProviderProfile
{
    // ===== Update Profile Request =====
    public class UpdateProviderProfileRequest
    {
        public string? Bio { get; set; }
        public string? Experience { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
