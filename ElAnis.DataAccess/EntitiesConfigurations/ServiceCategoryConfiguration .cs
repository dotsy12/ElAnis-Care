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
    public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
    {
        public void Configure(EntityTypeBuilder<ServiceCategory> builder)
        {
            builder.ToTable("ServiceCategories");

            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(sc => sc.Description)
                   .HasMaxLength(500);

            builder.Property(sc => sc.BasePrice)
                   .HasPrecision(10, 2);

            builder.Property(sc => sc.DisplayOrder)
                   .HasDefaultValue(0);

            builder.Property(sc => sc.IsActive)
                   .HasDefaultValue(true);

            builder.Property(sc => sc.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
