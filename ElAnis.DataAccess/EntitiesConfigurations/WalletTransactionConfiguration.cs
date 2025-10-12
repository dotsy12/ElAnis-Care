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
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.Type)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(t => t.Wallet)
                   .WithMany(w => w.Transactions)
                   .HasForeignKey(t => t.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
