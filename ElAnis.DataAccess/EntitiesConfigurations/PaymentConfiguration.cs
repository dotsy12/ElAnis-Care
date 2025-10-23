using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        // ===== Primary Key =====
        builder.HasKey(p => p.Id);

        // ===== Properties =====
        builder.Property(p => p.Amount)
               .IsRequired()
               .HasColumnType("decimal(18,2)"); // تحديد دقة القيمة المالية

        builder.Property(p => p.PaymentMethod)
               .IsRequired();

        builder.Property(p => p.PaymentStatus)
               .IsRequired()
               .HasDefaultValue(PaymentStatus.Pending); // Pending

        builder.Property(p => p.TransactionId)
               .HasMaxLength(200);

        builder.Property(p => p.PaymentGatewayResponse)
               .HasColumnType("nvarchar(max)");

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // ===== Relationships =====
        builder.HasOne(p => p.ServiceRequest)
               .WithOne(sr => sr.Payment)
               .HasForeignKey<Payment>(p => p.ServiceRequestId)
               .OnDelete(DeleteBehavior.Cascade);

        // ===== Indexes =====
        builder.HasIndex(p => p.ServiceRequestId);
        builder.HasIndex(p => p.PaymentStatus);
        builder.HasIndex(p => p.CreatedAt);
    }
}
