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
    public class ServiceReportConfiguration : IEntityTypeConfiguration<ServiceReport>
    {
        public void Configure(EntityTypeBuilder<ServiceReport> builder)
        {
            

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReportType)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(r => r.ReportContent)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.Property(r => r.Attachments)
                   .HasMaxLength(2000);

            builder.Property(r => r.ClientAcknowledged)
                   .HasDefaultValue(false);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.Provider)
                   .WithMany()
                   .HasForeignKey(r => r.ProviderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ServiceExecution)
                   .WithMany(e => e.Reports)
                   .HasForeignKey(r => r.ServiceExecutionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
