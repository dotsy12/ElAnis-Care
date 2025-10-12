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
    public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
    {
        public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
        {
           

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(w => w.Status)
                   .HasConversion<string>()
                   .IsRequired();
            builder.Property(wr => wr.Status)
                  .HasConversion<string>()
                  .IsRequired();

            builder.Property(w => w.RequestedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(w => w.Wallet)
                   .WithMany()
                   .HasForeignKey(w => w.WalletId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
