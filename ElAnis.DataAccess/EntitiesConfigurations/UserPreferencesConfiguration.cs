// UserPreferencesConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ElAnis.Entities.Models;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
	public void Configure(EntityTypeBuilder<UserPreferences> builder)
	{
		builder.ToTable("UserPreferences");

		builder.HasKey(up => up.Id);

		builder.Property(up => up.PreferredLanguage)
			   .HasMaxLength(10)
			   .HasDefaultValue("ar");

		builder.Property(up => up.PreferredCurrency)
			   .HasMaxLength(10)
			   .HasDefaultValue("EGP");

		// Default values
		builder.Property(up => up.EmailNotifications)
			   .HasDefaultValue(true);

		builder.Property(up => up.PushNotifications)
			   .HasDefaultValue(true);

		builder.Property(up => up.SmsNotifications)
			   .HasDefaultValue(false);

		// Relationship
		builder.HasOne(up => up.User)
			   .WithOne(u => u.UserPreferences)
			   .HasForeignKey<UserPreferences>(up => up.UserId);
	}
}