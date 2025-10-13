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
    public class PriceRuleConfiguration : IEntityTypeConfiguration<PriceRule>
    {
        public void Configure(EntityTypeBuilder<PriceRule> builder)
        {
            builder.ToTable("price_rules");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.DurationType).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(p => p.PlatformCommissionPercent).HasDefaultValue(10.0m);

            builder.HasIndex(p => new { p.ServiceTypeId, p.ServiceCategoryId, p.IsActive });

            builder.HasMany(p => p.Tiers)
                   .WithOne()
                   .HasForeignKey(t => t.PriceRuleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ServiceType>()
                   .WithMany()
                   .HasForeignKey(p => p.ServiceTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ServiceCategory>()
                   .WithMany()
                   .HasForeignKey(p => p.ServiceCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
