using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using System.Text.Json;

namespace ElAnis.DataAccess.Configurations
{
	public class ServiceProviderApplicationEntityConfiguration : IEntityTypeConfiguration<ServiceProviderApplication>
	{
		public void Configure(EntityTypeBuilder<ServiceProviderApplication> builder)
		{
			builder.ToTable("ServiceProviderApplications");
			builder.HasKey(spa => spa.Id);

			// Properties with specific configurations
			builder.Property(spa => spa.FirstName)
				   .HasMaxLength(100)
				   .IsRequired();

			builder.Property(spa => spa.LastName)
				   .HasMaxLength(100)
				   .IsRequired();

			builder.Property(spa => spa.PhoneNumber)
				   .HasMaxLength(20)
				   .IsRequired();

			builder.Property(spa => spa.Address)
				   .HasMaxLength(500);

			builder.Property(spa => spa.Bio)
				   .HasMaxLength(1000);

			builder.Property(spa => spa.NationalId)
				   .HasMaxLength(20)
				   .IsRequired();

			builder.Property(spa => spa.Experience)
				   .HasMaxLength(2000);

			// Fix decimal precision
			builder.Property(spa => spa.HourlyRate)
				   .HasColumnType("decimal(18,2)")
				   .IsRequired();

			// Convert List<Guid> to JSON string for storage with value comparer
			builder.Property(e => e.SelectedCategories)
				.HasConversion(
					v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
					v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null) ?? new List<Guid>()
				)
				.HasColumnName("SelectedCategoriesJson")
				.HasColumnType("nvarchar(max)")
				.Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<Guid>>(
					(c1, c2) => c1.SequenceEqual(c2),
					c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
					c => c.ToList()));

			// Fix enum default value with sentinel
			builder.Property(spa => spa.Status)
				   .HasDefaultValue(ServiceProviderApplicationStatus.Pending)
				   .HasSentinel(ServiceProviderApplicationStatus.Pending);

			builder.Property(spa => spa.CreatedAt)
				   .HasDefaultValueSql("GETUTCDATE()");

			builder.Property(spa => spa.RejectionReason)
				   .HasMaxLength(1000);

			// Relationships
			builder.HasOne(spa => spa.User)
				   .WithMany()
				   .HasForeignKey(spa => spa.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(spa => spa.ReviewedBy)
				   .WithMany()
				   .HasForeignKey(spa => spa.ReviewedById)
				   .OnDelete(DeleteBehavior.SetNull);

			// Indexes
			builder.HasIndex(spa => spa.UserId);
			builder.HasIndex(spa => spa.Status);
			builder.HasIndex(spa => spa.CreatedAt);
			builder.HasIndex(spa => spa.NationalId).IsUnique();
		}
	}
}