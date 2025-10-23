using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeCare.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // ===== Properties =====
            builder.Property(u => u.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Address)
                .HasMaxLength(500);

            //builder.Property(u => u.Governorate)
            //    .HasMaxLength(100);

            builder.Property(u => u.ProfilePicture)
                .HasMaxLength(500);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // ===== Indexes =====
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.PhoneNumber).IsUnique();
            builder.HasIndex(u => u.IsActive);
            builder.HasIndex(u => u.CreatedAt);

            // ===== Relationships =====

            // One-to-One: User ↔ ServiceProviderProfile
            builder.HasOne(u => u.ServiceProviderProfile)
                .WithOne(sp => sp.User)
                .HasForeignKey<ServiceProviderProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // One-to-One: User ↔ ServiceProviderApplication
            builder.HasOne(u => u.ServiceProviderApplication)
                .WithOne(spa => spa.User)
                .HasForeignKey<ServiceProviderApplication>(spa => spa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        
            // One-to-Many: User ↔ ServiceRequests
            builder.HasMany(u => u.ServiceRequests)
                .WithOne(sr => sr.User)
                .HasForeignKey(sr => sr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // One-to-Many: User ↔ GivenReviews (Client)
            builder.HasMany(u => u.GivenReviews)
                .WithOne(r => r.Client)
                .HasForeignKey(r => r.ClientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: User ↔ ReceivedReviews (ServiceProvider)
            builder.HasMany(u => u.ReceivedReviews)
                .WithOne(r => r.ServiceProvider)
                .HasForeignKey(r => r.ServiceProviderUserId)
                .OnDelete(DeleteBehavior.Restrict);


            // One-to-Many: User ↔ Notifications
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
