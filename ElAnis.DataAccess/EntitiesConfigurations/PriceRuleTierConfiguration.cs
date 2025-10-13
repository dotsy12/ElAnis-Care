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
    public class PriceRuleTierConfiguration : IEntityTypeConfiguration<PriceRuleTier>
    {
        public void Configure(EntityTypeBuilder<PriceRuleTier> builder)
        {
            builder.ToTable("price_rule_tiers");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Price).IsRequired();
            builder.Property(t => t.FromValue).IsRequired();
            builder.Property(t => t.ToValue).IsRequired();
        }
    }
}
