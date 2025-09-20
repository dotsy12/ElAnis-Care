using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;

namespace ElAnis.DataAccess.Configurations
{
	public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
	{
		public void Configure(EntityTypeBuilder<ServiceRequest> builder)
		{
			builder.ToTable("ServiceRequests");

			builder.HasKey(sr => sr.Id);

			// Properties
			builder.Property(sr => sr.Description)
				   .IsRequired()
				   .HasMaxLength(1000);

			builder.Property(sr => sr.Address)
				   .IsRequired()
				   .HasMaxLength(500);

			// Fix decimal precision warning
			builder.Property(sr => sr.OfferedPrice)
				   .HasColumnType("decimal(18,2)");

			builder.Property(sr => sr.Status)
				   .HasDefaultValue(ServiceRequestStatus.Pending);

			// Relationships
			builder.HasOne(sr => sr.User)
				   .WithMany(u => u.ServiceRequests)
				   .HasForeignKey(sr => sr.UserId)
				   .OnDelete(DeleteBehavior.NoAction);


			builder.HasOne(sr => sr.Category)
				   .WithMany(c => c.ServiceRequests)
				   .HasForeignKey(sr => sr.CategoryId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(sr => sr.ServiceProvider)
				   .WithMany(sp => sp.ServiceRequests)
				   .HasForeignKey(sr => sr.ServiceProviderId)
				   .OnDelete(DeleteBehavior.SetNull);

			// Indexes
			builder.HasIndex(sr => sr.UserId);
			builder.HasIndex(sr => sr.CategoryId);
			builder.HasIndex(sr => sr.ServiceProviderId);
			builder.HasIndex(sr => sr.Status);
			builder.HasIndex(sr => sr.CreatedAt);
		}
	}
}