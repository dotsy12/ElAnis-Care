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
    public class ComplaintMessageConfiguration: IEntityTypeConfiguration<ComplaintMessage>
    {
        public void Configure(EntityTypeBuilder<ComplaintMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Message)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(m => m.AttachmentUrl)
                   .HasMaxLength(500);

            builder.Property(m => m.SenderType)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(m => m.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(m => m.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(m => m.Complaint)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ComplaintId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.SenderUser)
                   .WithMany()
                   .HasForeignKey(m => m.SenderUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

