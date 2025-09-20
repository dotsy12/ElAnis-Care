using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;

namespace ElAnis.DataAccess.Configurations
{
	public class ServiceProviderProfileEntityConfigurations : IEntityTypeConfiguration<ServiceProviderProfile>
	{
		public void Configure(EntityTypeBuilder<ServiceProviderProfile> builder)
		{
			builder.ToTable("ServiceProviderProfiles");

			builder.HasKey(sp => sp.Id);

			// Properties with specific configurations
			builder.Property(sp => sp.Bio)
				   .HasMaxLength(1000);

			builder.Property(sp => sp.Experience)
				   .HasMaxLength(2000);

			// Fix decimal precision warnings
			builder.Property(sp => sp.HourlyRate)
				   .HasColumnType("decimal(18,2)")
				   .IsRequired();

			builder.Property(sp => sp.TotalEarnings)
				   .HasColumnType("decimal(18,2)")
				   .HasDefaultValue(0m);

			// Fix enum default value warning
			builder.Property(sp => sp.Status)
				   .HasDefaultValue(ServiceProviderStatus.Pending);

			builder.Property(sp => sp.IsAvailable)
				   .HasDefaultValue(true);

			// Relationships
			builder.HasOne(sp => sp.User)
				   .WithOne(u => u.ServiceProviderProfile)
				   .HasForeignKey<ServiceProviderProfile>(sp => sp.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			// Indexes for better performance
			builder.HasIndex(sp => sp.UserId).IsUnique();
			builder.HasIndex(sp => sp.Status);
			builder.HasIndex(sp => sp.IsAvailable);
			builder.HasIndex(sp => sp.AverageRating);
		}
	}
}