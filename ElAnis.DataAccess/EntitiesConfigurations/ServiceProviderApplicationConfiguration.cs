using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class ServiceProviderApplicationConfiguration : IEntityTypeConfiguration<ServiceProviderApplication>
{
    public void Configure(EntityTypeBuilder<ServiceProviderApplication> builder)
    {
        // اسم الجدول
        builder.ToTable("ServiceProviderApplications");

        // المفتاح الأساسي
        builder.HasKey(s => s.Id);

        // ===== Properties =====
        builder.Property(s => s.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(s => s.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(s => s.PhoneNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(s => s.Address)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(s => s.NationalId)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(s => s.Bio)
               .HasMaxLength(1000);

        builder.Property(s => s.Experience)
               .HasMaxLength(2000);

        builder.Property(sp => sp.HourlyRate)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(s => s.IdDocumentPath)
               .HasMaxLength(500);

        builder.Property(s => s.CertificatePath)
               .HasMaxLength(500);

        builder.Property(s => s.CVPath)
               .HasMaxLength(500);

        // 📝 ملاحظة: SelectedCategories مش مدعومة مباشرة، 
        // لو هتخزنها كـ JSON string لازم تضيف Conversion هنا (optional)
        builder.Property(s => s.SelectedCategories)
               .HasConversion(
                   v => string.Join(',', v),         // من List<Guid> إلى string
                   v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(Guid.Parse)
                         .ToList()
               )
               .HasColumnName("SelectedCategoryIds")
               .HasMaxLength(2000);

        builder.Property(s => s.Status)
               .IsRequired();

        builder.Property(s => s.RejectionReason)
               .HasMaxLength(1000);

        builder.Property(s => s.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // ===== Relationships =====

        // User الأساسي المرتبط بالتقديم
        builder.HasOne(s => s.User)
               .WithOne(u => u.ServiceProviderApplication)
               .HasForeignKey<ServiceProviderApplication>(s => s.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        // User اللي راجع الطلب (لو موجود)
        builder.HasOne(s => s.ReviewedBy)
               .WithMany()
               .HasForeignKey(s => s.ReviewedById)
               .OnDelete(DeleteBehavior.SetNull);

        // ===== Indexes =====
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.CreatedAt);
    }
}
