using ElAnis.Entities.DTO.Account.Auth.Register;
using FluentValidation;
using System;
using System.IO;
using System.Linq;

namespace ElAnis.API.Validators
{
    public class RegisterServiceProviderRequestValidator : AbstractValidator<RegisterServiceProviderRequest>
    {
        private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

        public RegisterServiceProviderRequestValidator()
        {
            // =========================
            // 📌 1. Basic Account Info
            // =========================
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must be between 10 and 15 digits.");

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

            // =========================
            // 👤 2. Personal Information
            // =========================
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").WithMessage("First name can only contain letters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
                .Matches(@"^[a-zA-Z\u0600-\u06FF\s]+$").WithMessage("Last name can only contain letters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(BeValidAge).WithMessage("Age must be between 18 and 65 years.");

            // =========================
            // 💼 3. Professional Information
            // =========================
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

            // =========================
            // 📎 4. File Validation
            // =========================
            RuleFor(x => x.IdDocument)
                .NotNull().WithMessage("ID document is required.")
                .Must(BeValidFileSize).WithMessage("ID document size cannot exceed 5MB.")
                .Must(BeValidFileType).WithMessage("ID document must be PDF, JPG, or PNG.");

            RuleFor(x => x.Certificate)
                .NotNull().WithMessage("Certificate is required.")
                .Must(BeValidFileSize).WithMessage("Certificate size cannot exceed 5MB.")
                .Must(BeValidFileType).WithMessage("Certificate must be PDF, JPG, or PNG.");

            RuleFor(x => x.CV)
                .NotNull().WithMessage("CV is required.")
                .Must(BeValidFileSize).WithMessage("CV size cannot exceed 5MB.")
                .Must(BeValidFileType).WithMessage("CV must be PDF, JPG, or PNG.");

            // =========================
            // 🏷 5. Categories
            // =========================
            RuleFor(x => x.SelectedCategoryIds)
                .NotEmpty().WithMessage("At least one category must be selected.")
                .Must(ids => ids.All(id => id != Guid.Empty))
                .WithMessage("Category IDs must be valid GUIDs.");
        }

        private bool BeValidAge(DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
            return age >= 18 && age <= 65;
        }

        private bool BeValidFileSize(IFormFile? file)
        {
            if (file == null) return true;
            return file.Length <= MaxFileSize;
        }

        private bool BeValidFileType(IFormFile? file)
        {
            if (file == null) return true;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
    }
}
