using ElAnis.Entities.Models.Auth.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Entities.Models
{
    public class FavoriteProvider
    {
        public Guid Id { get; set; }

        public string ClientUserId { get; set; } = null!;
        public User ClientUser { get; set; } = null!;

        public string ProviderUserId { get; set; } = null!;
        public User ProviderUser { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
