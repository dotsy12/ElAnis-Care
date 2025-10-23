using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class ProviderAvailabilityConfiguration : IEntityTypeConfiguration<ProviderAvailability>
{
    public void Configure(EntityTypeBuilder<ProviderAvailability> builder)
    {
        builder.ToTable("ProviderAvailabilities");

        // ===== Primary Key =====
        builder.HasKey(pa => pa.Id);

        // ===== Properties =====
        builder.Property(pa => pa.Date)
               .IsRequired();

        builder.Property(pa => pa.IsAvailable)
               .IsRequired()
               .HasDefaultValue(true);

        builder.Property(pa => pa.AvailableShift)
               .HasConversion<int>() // تخزين الـ Enum كـ int
               .IsRequired(false);

        builder.Property(pa => pa.Notes)
               .HasMaxLength(500);

        builder.Property(pa => pa.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // ===== Relationships =====
        builder.HasOne(pa => pa.ServiceProvider)
               .WithMany(sp => sp.Availability)
               .HasForeignKey(pa => pa.ServiceProviderId)
               .OnDelete(DeleteBehavior.Cascade);

        // ===== Indexes =====
        builder.HasIndex(pa => pa.ServiceProviderId);
        builder.HasIndex(pa => pa.Date);
        builder.HasIndex(pa => pa.IsAvailable);
    }
}
