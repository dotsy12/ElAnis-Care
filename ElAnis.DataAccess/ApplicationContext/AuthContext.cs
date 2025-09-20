using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models.Auth.UserTokens;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System.Reflection;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.ApplicationContext
{
    public class AuthContext : IdentityDbContext<User, Role, string>, IDataProtectionKeyContext
    {
        public AuthContext(DbContextOptions<AuthContext> options)
		: base(options)
        {
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Apply configurations
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}

		public DbSet<ServiceProviderProfile> ServiceProviderProfiles { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ServiceProviderCategory> ServiceProviderCategories { get; set; }
		public DbSet<ServiceRequest> ServiceRequests { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
		public DbSet<ServiceProviderApplication> ServiceProviderApplications { get; set; }
		public DbSet<UserPreferences> UserPreferences { get; set; }

		public DbSet<Notification> Notifications { get; set; }
	}
}
