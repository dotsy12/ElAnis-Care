using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // اسم الجدول
            builder.ToTable("payments");

            // المفتاح الأساسي
            builder.HasKey(p => p.Id);

            // العمود الأساسي
            builder.Property(p => p.Id)
                   .HasColumnName("id");

            // العلاقة مع العميل (Client)
            builder.HasOne(p => p.Client)
                   .WithMany()
                   .HasForeignKey(p => p.ClientUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع مقدم الخدمة (Provider)
            builder.HasOne(p => p.Provider)
                   .WithMany()
                   .HasForeignKey(p => p.ProviderUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // العلاقة مع طلب الخدمة (ServiceRequest)
            builder.HasOne(p => p.ServiceRequest)
                   .WithMany()
                   .HasForeignKey(p => p.ServiceRequestId)
                   .OnDelete(DeleteBehavior.SetNull);

            // العمود: Amount
            builder.Property(p => p.Amount)
                   .HasPrecision(18, 2) // دقة مالية
                   .IsRequired();

            // العمود: Method
            builder.Property(p => p.Method)
                   .HasConversion<string>() // يخزن النص بدل رقم (مثلاً "CreditCard")
                   .IsRequired();

            // العمود: Status
            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .IsRequired();

            // العمود: CreatedAt
            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()")
                   .IsRequired();
        }
    }
}
