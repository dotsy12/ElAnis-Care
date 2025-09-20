using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
	public class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
	{
		public void Configure(EntityTypeBuilder<Review> builder)
		{
			builder.HasKey(r => r.Id);

			builder.Property(r => r.Rating)
				   .IsRequired();

			// التأكد من أن التقييم بين 1 و 5
			builder.HasCheckConstraint("CK_Review_Rating", "Rating >= 1 AND Rating <= 5");

			builder.Property(r => r.Comment)
				   .HasMaxLength(1000);

			builder.Property(r => r.CreatedAt)
				   .HasDefaultValueSql("GETUTCDATE()");

			// تحديد العلاقات بوضوح
			// العلاقة مع العميل (اللي عامل التقييم)
			builder.HasOne(r => r.Client)
				   .WithMany(u => u.GivenReviews)
				   .HasForeignKey(r => r.ClientUserId)
				   .OnDelete(DeleteBehavior.Restrict)
				   .HasConstraintName("FK_Review_Client");

			// العلاقة مع مقدم الخدمة (اللي باخد التقييم)
			builder.HasOne(r => r.ServiceProvider)
				   .WithMany(u => u.ReceivedReviews)
				   .HasForeignKey(r => r.ServiceProviderUserId)
				   .OnDelete(DeleteBehavior.Restrict)
				   .HasConstraintName("FK_Review_ServiceProvider");

			// العلاقة مع طلب الخدمة
			builder.HasOne(r => r.ServiceRequest)
				   .WithMany()
				   .HasForeignKey(r => r.ServiceRequestId)
				   .OnDelete(DeleteBehavior.Cascade);

			// فهارس للبحث السريع
			builder.HasIndex(r => r.ClientUserId)
				   .HasDatabaseName("IX_Review_ClientUserId");

			builder.HasIndex(r => r.ServiceProviderUserId)
				   .HasDatabaseName("IX_Review_ServiceProviderUserId");

			builder.HasIndex(r => r.Rating)
				   .HasDatabaseName("IX_Review_Rating");
		}
	}
}
