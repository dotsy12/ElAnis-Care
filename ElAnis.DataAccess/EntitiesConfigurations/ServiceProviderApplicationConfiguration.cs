using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class ServiceProviderApplicationConfiguration : IEntityTypeConfiguration<ServiceProviderApplication>
{
	public void Configure(EntityTypeBuilder<ServiceProviderApplication> builder)
	{
		builder.ToTable("ServiceProviderApplications");
		builder.HasKey(s => s.Id);

		// Properties
		builder.Property(s => s.FirstName)
			   .IsRequired()
			   .HasMaxLength(100);

		builder.Property(s => s.LastName)
			   .IsRequired()
			   .HasMaxLength(100);

		builder.Property(s => s.Bio)
			   .HasMaxLength(1000);

		builder.Property(s => s.Experience)
			   .HasMaxLength(2000);

		builder.Property(sp => sp.HourlyRate)
		   .HasColumnType("decimal(18,2)")
		   .IsRequired();
		// Relationships
		builder.HasOne(s => s.User)
			   .WithOne(u => u.ServiceProviderApplication)
			   .HasForeignKey<ServiceProviderApplication>(s => s.UserId)
			   .OnDelete(DeleteBehavior.NoAction); // Changed from default Cascade to NoAction

		builder.HasOne(s => s.ReviewedBy)
			   .WithMany()
			   .HasForeignKey(s => s.ReviewedById)
			   .OnDelete(DeleteBehavior.SetNull);
	}
}