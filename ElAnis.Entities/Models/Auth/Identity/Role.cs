using Microsoft.AspNetCore.Identity;

namespace ElAnis.Entities.Models.Auth.Identity
{
    public class Role : IdentityRole<string>
    {
		
			public string? Description { get; set; }
			public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		
	}
}
