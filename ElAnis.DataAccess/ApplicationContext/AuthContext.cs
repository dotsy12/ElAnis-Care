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


            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.ServiceRequest)
                    .WithMany()
                    .HasForeignKey(c => c.ServiceRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ServiceProvider)
                    .WithMany()
                    .HasForeignKey(c => c.ServiceProviderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.ServiceRequestId).IsUnique();
            });

            // ChatMessage Configuration
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasOne(m => m.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                    .WithMany()
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => new { m.ChatId, m.SentAt });
            });

            // UserConnection Configuration
            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.UserId);
                entity.HasIndex(u => u.ConnectionId).IsUnique();
            });
        }

		public DbSet<ServiceProviderProfile> ServiceProviderProfiles { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<ServiceProviderCategory> ServiceProviderCategories { get; set; }
		public DbSet<ServiceRequest> ServiceRequests { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
		public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
		public DbSet<ServiceProviderApplication> ServiceProviderApplications { get; set; } 

	    public DbSet <Payment> payments { get; set; }
		public DbSet <ProviderAvailability> ProviderAvailabilities { get; set; }
		public DbSet <ProviderWorkingArea> ProviderWorkingAreas { get; set; }
		public DbSet <ServicePricing> ServicePricings { get; set; }

		public DbSet<Notification> Notifications { get; set; }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
    }
}
