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
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("ServiceTypes");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(st => st.Description)
                   .HasMaxLength(500);

            builder.Property(st => st.IconUrl)
                   .HasMaxLength(300);

            builder.Property(st => st.IsActive)
                   .HasDefaultValue(true);

            builder.Property(st => st.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasMany(st => st.Categories)
                   .WithOne(c => c.ServiceType)
                   .HasForeignKey(c => c.ServiceTypeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
