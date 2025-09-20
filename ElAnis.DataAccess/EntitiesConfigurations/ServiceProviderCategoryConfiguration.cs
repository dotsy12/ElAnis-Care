using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ElAnis.Entities.Models;

public class ServiceProviderCategoryConfiguration : IEntityTypeConfiguration<ServiceProviderCategory>
{
	public void Configure(EntityTypeBuilder<ServiceProviderCategory> builder)
	{
		builder.ToTable("ServiceProviderCategories");

		builder.HasKey(sc => new { sc.ServiceProviderId, sc.CategoryId });

		// علاقة مع ServiceProviderProfile
		builder.HasOne(sc => sc.ServiceProvider)
			   .WithMany(sp => sp.Categories)
			   .HasForeignKey(sc => sc.ServiceProviderId);

		// علاقة مع Category
		builder.HasOne(sc => sc.Category)
			   .WithMany(c => c.ServiceProviders)
			   .HasForeignKey(sc => sc.CategoryId);
	}
}
