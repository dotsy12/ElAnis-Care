
using ElAnis.Entities.DTO.Account.Auth.Register;
using FluentValidation;

namespace ElAnis.API.Validators
{
	public class RegisterServiceProviderRequestValidator : AbstractValidator<RegisterServiceProviderRequest>
	{
		public RegisterServiceProviderRequestValidator()
		{
			// Basic Account Info
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Email must be valid (e.g., provider@example.com).");

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage("Phone number is required.")
				.Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must contain only digits and be between 10 and 15 characters.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
				.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
				.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
				.Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
				.Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character (!@#$%^&*).");

			RuleFor(x => x.ConfirmPassword)
				.NotEmpty().WithMessage("Confirm password is required.")
				.Equal(x => x.Password).WithMessage("Password and confirm password must match.");

			// Personal Information
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required.")
				.MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
				.Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").WithMessage("First name can only contain letters and spaces.");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required.")
				.MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
				.Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").WithMessage("Last name can only contain letters and spaces.");

			RuleFor(x => x.Address)
				.NotEmpty().WithMessage("Address is required.")
				.MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

			RuleFor(x => x.DateOfBirth)
				.NotEmpty().WithMessage("Date of birth is required.")
				.Must(BeValidAge).WithMessage("Age must be between 18 and 65 years.");

			// Professional Information
			RuleFor(x => x.Bio)
				.NotEmpty().WithMessage("Bio is required.")
				.MinimumLength(50).WithMessage("Bio must be at least 50 characters.")
				.MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.");

			RuleFor(x => x.NationalId)
				.NotEmpty().WithMessage("National ID is required.")
				.Matches(@"^\d{14}$").WithMessage("National ID must be exactly 14 digits.");

			RuleFor(x => x.Experience)
				.NotEmpty().WithMessage("Experience is required.")
				.MinimumLength(20).WithMessage("Experience must be at least 20 characters.")
				.MaximumLength(2000).WithMessage("Experience cannot exceed 2000 characters.");

			RuleFor(x => x.HourlyRate)
				.NotEmpty().WithMessage("Hourly rate is required.")
				.GreaterThan(0).WithMessage("Hourly rate must be greater than 0.")
				.LessThanOrEqualTo(10000).WithMessage("Hourly rate cannot exceed 10,000.");

			// Selected Categories
			RuleFor(x => x.SelectedCategoryIds)
				.NotEmpty().WithMessage("At least one category must be selected.")
				.Must(BeValidCategoryIds).WithMessage("Category IDs must be valid numbers separated by commas.");

			// File Validation
			RuleFor(x => x.IdDocument)
				.NotNull().WithMessage("ID document is required.")
				.Must(BeValidFileSize).WithMessage("ID document size cannot exceed 5MB.")
				.Must(BeValidFileType).WithMessage("ID document must be PDF, JPG, or PNG.");

			RuleFor(x => x.Certificate)
				.Must(BeValidFileSize).WithMessage("Certificate size cannot exceed 5MB.")
				.Must(BeValidFileType).WithMessage("Certificate must be PDF, JPG, or PNG.")
				.When(x => x.Certificate != null);
		}

		private bool BeValidAge(DateTime dateOfBirth)
		{
			var age = DateTime.Today.Year - dateOfBirth.Year;
			if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

			return age >= 18 && age <= 65;
		}

		private bool BeValidCategoryIds(string categoryIds)
		{
			if (string.IsNullOrWhiteSpace(categoryIds)) return false;

			var ids = categoryIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
			if (ids.Length == 0) return false;

			foreach (var id in ids)
			{
				if (!int.TryParse(id.Trim(), out var categoryId) || categoryId <= 0)
					return false;
			}

			return true;
		}

		private bool BeValidFileSize(Microsoft.AspNetCore.Http.IFormFile? file)
		{
			if (file == null) return true;
			return file.Length <= 5 * 1024 * 1024; // 5MB
		}

		private bool BeValidFileType(Microsoft.AspNetCore.Http.IFormFile? file)
		{
			if (file == null) return true;

			var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

			return allowedExtensions.Contains(extension);
		}
	}
}