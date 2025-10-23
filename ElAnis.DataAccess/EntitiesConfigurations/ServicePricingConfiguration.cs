using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
    public class ServicePricingConfiguration : IEntityTypeConfiguration<ServicePricing>
    {
        public void Configure(EntityTypeBuilder<ServicePricing> builder)
        {
            // اسم الجدول
            builder.ToTable("ServicePricings");

            // المفتاح الأساسي
            builder.HasKey(sp => sp.Id);

            // العلاقة مع Category (كل سعر مرتبط بفئة واحدة)
            builder.HasOne(sp => sp.Category)
                   .WithMany(c => c.Pricing)
                   .HasForeignKey(sp => sp.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_ServicePricing_Category");

            // الحقول
            builder.Property(sp => sp.ShiftType)
                   .IsRequired();

            builder.Property(sp => sp.PricePerShift)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(sp => sp.Description)
                   .HasMaxLength(500);

            builder.Property(sp => sp.IsActive)
                   .HasDefaultValue(true);

            builder.Property(sp => sp.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(sp => sp.UpdatedBy)
                   .HasMaxLength(200);

            // ✅ إضافة Indexات لتحسين الأداء (لو بتبحث حسب Category أو ShiftType)
            builder.HasIndex(sp => new { sp.CategoryId, sp.ShiftType })
                   .HasDatabaseName("IX_ServicePricing_Category_ShiftType");
        }
    }
}
