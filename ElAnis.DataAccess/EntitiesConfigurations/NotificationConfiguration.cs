// NotificationConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
	public void Configure(EntityTypeBuilder<Notification> builder)
	{
		builder.ToTable("Notifications");

		builder.HasKey(n => n.Id);

		builder.Property(n => n.Title)
			   .IsRequired()
			   .HasMaxLength(200);

		builder.Property(n => n.Message)
			   .IsRequired()
			   .HasMaxLength(1000);

		// Relationships
		builder.HasOne(n => n.User)
			   .WithMany(u => u.Notifications)
			   .HasForeignKey(n => n.UserId);

		builder.HasOne(n => n.ServiceRequest)
			   .WithMany()
			   .HasForeignKey(n => n.ServiceRequestId)
			   .OnDelete(DeleteBehavior.SetNull);

		// Indexes for performance
		builder.HasIndex(n => new { n.UserId, n.IsRead });
		builder.HasIndex(n => n.CreatedAt);
	}
}
