using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("reviews");

            builder.HasKey(r => r.Id);

            // ✅ التقييم من 1 إلى 5 فقط
            builder.Property(r => r.Rating)
                   .IsRequired()
                   .HasPrecision(2, 1); // يسمح بقيم مثل 4.5

            // ✅ قيد في قاعدة البيانات للتحقق من القيم (1 ≤ Rating ≤ 5)
            builder.HasCheckConstraint("CK_Reviews_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5");

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000);

            builder.Property(r => r.Tags)
                   .HasMaxLength(1000);

            builder.Property(r => r.IsVerified)
                   .HasDefaultValue(true);

            builder.Property(r => r.IsVisible)
                   .HasDefaultValue(true);

            builder.Property(r => r.AdminHidden)
                   .HasDefaultValue(false);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.ReviewerType)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // العلاقة مع الطلب
            builder.HasOne(r => r.ServiceRequest)
                   .WithMany()
                   .HasForeignKey(r => r.ServiceRequestId)
                   .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع المستخدمين
            builder.HasOne(r => r.ReviewerUser)
                   .WithMany()
                   .HasForeignKey(r => r.ReviewerUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReviewedUser)
                   .WithMany()
                   .HasForeignKey(r => r.ReviewedUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
