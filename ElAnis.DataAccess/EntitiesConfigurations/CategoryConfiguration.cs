using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Description)
               .HasMaxLength(500);

        builder.Property(c => c.Icon)
               .HasMaxLength(250);

        builder.Property(c => c.IsActive)
               .HasDefaultValue(true);

        builder.Property(c => c.DisplayOrder)
               .HasDefaultValue(0);

        builder.Property(c => c.CreatedAt)
               .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.DisplayOrder);

        // Relationships
        builder.HasMany(c => c.Pricing)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.ServiceProviders)
               .WithOne(spc => spc.Category)
               .HasForeignKey(spc => spc.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.ServiceRequests)
               .WithOne(sr => sr.Category)
               .HasForeignKey(sr => sr.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
