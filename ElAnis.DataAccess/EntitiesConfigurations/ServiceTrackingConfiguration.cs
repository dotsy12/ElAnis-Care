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
    public class ServiceTrackingConfiguration : IEntityTypeConfiguration<ServiceTracking>
    {
        public void Configure(EntityTypeBuilder<ServiceTracking> builder)
        {
            

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Latitude)
                   .HasColumnType("decimal(9,6)")
                   .IsRequired();

            builder.Property(t => t.Longitude)
                   .HasColumnType("decimal(9,6)")
                   .IsRequired();

            builder.Property(t => t.TrackingStatus)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(t => t.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(t => t.Provider)
                   .WithMany()
                   .HasForeignKey(t => t.ProviderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ServiceExecution)
                   .WithMany(e => e.Trackings)
                   .HasForeignKey(t => t.ServiceExecutionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
