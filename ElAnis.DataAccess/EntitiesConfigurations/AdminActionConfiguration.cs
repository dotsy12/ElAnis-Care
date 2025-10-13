using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
    public class AdminActionConfiguration : IEntityTypeConfiguration<AdminAction>
    {
        public void Configure(EntityTypeBuilder<AdminAction> builder)
        {
            builder.ToTable("admin_actions");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ActionType)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(a => a.TargetType)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(a => a.ActionDetails)
                   .HasColumnType("nvarchar(max)");

            builder.Property(a => a.IpAddress)
                   .HasMaxLength(50);

            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(a => new { a.AdminUserId, a.ActionType, a.CreatedAt });

            builder.HasOne(a => a.AdminUser)
                   .WithMany()
                   .HasForeignKey(a => a.AdminUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
