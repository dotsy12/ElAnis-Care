using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class ProviderWorkingAreaConfiguration : IEntityTypeConfiguration<ProviderWorkingArea>
{
    public void Configure(EntityTypeBuilder<ProviderWorkingArea> builder)
    {
        builder.ToTable("ProviderWorkingAreas");

        // ===== Primary Key =====
        builder.HasKey(pwa => pwa.Id);

        // ===== Properties =====
        builder.Property(pwa => pwa.Governorate)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(pwa => pwa.City)
               .HasMaxLength(100);

        builder.Property(pwa => pwa.District)
               .HasMaxLength(100);

        builder.Property(pwa => pwa.IsActive)
               .HasDefaultValue(true);

        builder.Property(pwa => pwa.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // ===== Relationships =====
        builder.HasOne(pwa => pwa.ServiceProvider)
               .WithMany(sp => sp.WorkingAreas)
               .HasForeignKey(pwa => pwa.ServiceProviderId)
               .OnDelete(DeleteBehavior.Cascade);

        // ===== Indexes =====
        builder.HasIndex(pwa => pwa.ServiceProviderId);
        builder.HasIndex(pwa => new { pwa.Governorate, pwa.City });
        builder.HasIndex(pwa => pwa.IsActive);
    }
}
