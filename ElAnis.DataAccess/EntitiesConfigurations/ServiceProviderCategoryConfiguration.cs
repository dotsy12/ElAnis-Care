using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class ServiceProviderCategoryConfiguration : IEntityTypeConfiguration<ServiceProviderCategory>
{
    public void Configure(EntityTypeBuilder<ServiceProviderCategory> builder)
    {
        // اسم الجدول
        builder.ToTable("ServiceProviderCategories");

        // المفتاح المركب (Composite Key)
        builder.HasKey(sc => new { sc.ServiceProviderId, sc.CategoryId });

        // ===== Properties =====
        builder.Property(sc => sc.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // ===== Relationships =====

        // علاقة مع ServiceProviderProfile (Many-to-Many)
        builder.HasOne(sc => sc.ServiceProvider)
               .WithMany(sp => sp.Categories)
               .HasForeignKey(sc => sc.ServiceProviderId)
               .OnDelete(DeleteBehavior.Cascade);

        // علاقة مع Category (Many-to-Many)
        builder.HasOne(sc => sc.Category)
               .WithMany(c => c.ServiceProviders)
               .HasForeignKey(sc => sc.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

        // ===== Indexes =====
        builder.HasIndex(sc => sc.ServiceProviderId)
               .HasDatabaseName("IX_ServiceProviderCategory_ServiceProviderId");

        builder.HasIndex(sc => sc.CategoryId)
               .HasDatabaseName("IX_ServiceProviderCategory_CategoryId");
    }
}
