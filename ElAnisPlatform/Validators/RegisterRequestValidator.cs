using ElAnis.Entities.DTO.Account.Auth.Register;
using FluentValidation;

namespace ElAnis.API.Validators
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Email must be valid (e.g., user@example.com).");

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

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required.")
				.MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
				.Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").WithMessage("First name can only contain letters and spaces.");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required.")
				.MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
				.Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").WithMessage("Last name can only contain letters and spaces.");

			RuleFor(x => x.Address)
				.MaximumLength(500).WithMessage("Address cannot exceed 500 characters.")
				.When(x => !string.IsNullOrEmpty(x.Address));

			RuleFor(x => x.DateOfBirth)
				.Must(BeValidAge).WithMessage("Age must be between 18 and 100 years.")
				.When(x => x.DateOfBirth.HasValue);
		}

		private bool BeValidAge(DateTime? dateOfBirth)
		{
			if (!dateOfBirth.HasValue) return true;

			var age = DateTime.Today.Year - dateOfBirth.Value.Year;
			if (dateOfBirth.Value.Date > DateTime.Today.AddYears(-age)) age--;

			return age >= 18 && age <= 100;
		}
	}
}

