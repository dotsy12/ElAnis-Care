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
    public class FavoriteProviderConfiguration : IEntityTypeConfiguration<FavoriteProvider>
    {
        public void Configure(EntityTypeBuilder<FavoriteProvider> builder)
        {
            builder.ToTable("FavoriteProviders");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.AddedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // العلاقة مع المستخدمين
            builder.HasOne(f => f.ClientUser)
                   .WithMany()
                   .HasForeignKey(f => f.ClientUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.ProviderUser)
                   .WithMany()
                   .HasForeignKey(f => f.ProviderUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // منع التكرار (عميل لا يضيف نفس المقدم مرتين)
            builder.HasIndex(f => new { f.ClientUserId, f.ProviderUserId })
                   .IsUnique();
        }
    }
}
