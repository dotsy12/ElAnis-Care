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
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            builder.Property(w => w.OwnerType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(w => w.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(w => w.User)
                   .WithMany()
                   .HasForeignKey(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
