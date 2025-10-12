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
    public class ServiceExecutionConfiguration : IEntityTypeConfiguration<ServiceExecution>
    {
        public void Configure(EntityTypeBuilder<ServiceExecution> builder)
        {
            

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Status)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(e => e.CheckInLocation)
                   .HasMaxLength(255);

            builder.Property(e => e.CheckOutLocation)
                   .HasMaxLength(255);

            builder.Property(e => e.Notes)
                   .HasMaxLength(2000);

            builder.Property(e => e.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(e => e.Provider)
                   .WithMany()
                   .HasForeignKey(e => e.ProviderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Reports)
                   .WithOne(r => r.ServiceExecution)
                   .HasForeignKey(r => r.ServiceExecutionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Trackings)
                   .WithOne(t => t.ServiceExecution)
                   .HasForeignKey(t => t.ServiceExecutionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

