using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeCare.DataAccess.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");

			builder.Property(u => u.FirstName)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(u => u.LastName)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(u => u.Address)
				.HasMaxLength(500);

			builder.Property(u => u.CreatedAt)
				.HasDefaultValueSql("GETUTCDATE()");

			// Indexes
			builder.HasIndex(u => u.Email).IsUnique();
			builder.HasIndex(u => u.PhoneNumber).IsUnique();
			builder.HasIndex(u => u.IsActive);
			builder.HasIndex(u => u.CreatedAt);

			// Relationships
			builder.HasOne(u => u.ServiceProviderProfile)
				.WithOne(sp => sp.User)
				.HasForeignKey<ServiceProviderProfile>(sp => sp.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			//builder.HasMany(u => u.ServiceRequests)
			//	.WithOne(sr => sr.User)
			//	.HasForeignKey(sr => sr.UserId)
			//	.OnDelete(DeleteBehavior.Restrict);س
		}
	}
}