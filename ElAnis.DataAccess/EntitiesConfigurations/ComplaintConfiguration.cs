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
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {


            builder.HasKey(c => c.Id);

            builder.Property(c => c.ComplaintNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(c => c.ComplaintNumber).IsUnique();
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => new { c.ComplainantUserId, c.Status });

            builder.Property(c => c.Subject)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(c => c.Description)
                   .IsRequired();

            builder.Property(c => c.Priority)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(c => c.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(c => c.ComplaintType)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(c => c.ResolutionAction)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // relations
            builder.HasOne(c => c.ComplainantUser)
                .WithMany()
                .HasForeignKey(c => c.ComplainantUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.AgainstUser)
                .WithMany()
                .HasForeignKey(c => c.AgainstUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.AssignedToUser)
                .WithMany()
                .HasForeignKey(c => c.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.ServiceRequest)
                .WithMany()
                .HasForeignKey(c => c.ServiceRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // navigation collections
            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Complaint)
                .HasForeignKey(m => m.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Evidence)
                .WithOne(e => e.Complaint)
                .HasForeignKey(e => e.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Activities)
                .WithOne(a => a.Complaint)
                .HasForeignKey(a => a.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Participants)
                .WithOne(p => p.Complaint)
                .HasForeignKey(p => p.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
    
