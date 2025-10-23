using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
    public class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // المفتاح الأساسي
            builder.HasKey(r => r.Id);

            // التقييم
            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.HasCheckConstraint("CK_Review_Rating", "Rating >= 1 AND Rating <= 5");

            // التعليق
            builder.Property(r => r.Comment)
                   .HasMaxLength(1000);

            // وقت الإنشاء (UTC)
            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .ValueGeneratedOnAdd();

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

            builder.HasOne(r => r.ServiceRequest)
        .WithOne(sr => sr.Review)
        .HasForeignKey<Review>(r => r.ServiceRequestId)
        .OnDelete(DeleteBehavior.Cascade)
        .HasConstraintName("FK_Review_ServiceRequest");


            // الفهارس
            builder.HasIndex(r => r.ClientUserId)
                   .HasDatabaseName("IX_Review_ClientUserId");

            builder.HasIndex(r => r.ServiceProviderUserId)
                   .HasDatabaseName("IX_Review_ServiceProviderUserId");

            builder.HasIndex(r => r.Rating)
                   .HasDatabaseName("IX_Review_Rating");
        }
    }
}
