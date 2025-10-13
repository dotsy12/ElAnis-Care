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
    public class PlatformSettingHistoryConfiguration : IEntityTypeConfiguration<PlatformSettingHistory>
    {
        public void Configure(EntityTypeBuilder<PlatformSettingHistory> builder)
        {
            builder.ToTable("platform_setting_history");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.OldValue).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(h => h.NewValue).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(h => h.ChangedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasIndex(h => new { h.SettingId, h.ChangedAt });
        }
    }
}
