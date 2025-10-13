using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.EntitiesConfigurations
{
    public class PlatformSettingConfiguration : IEntityTypeConfiguration<PlatformSetting>
    {
        public void Configure(EntityTypeBuilder<PlatformSetting> builder)
        {
            builder.ToTable("platform_settings");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Key).HasMaxLength(200).IsRequired();
            builder.HasIndex(s => s.Key).IsUnique();
            builder.Property(s => s.Value).IsRequired().HasColumnType("nvarchar(max)");
            builder.Property(s => s.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(s => s.UpdatedByUserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
