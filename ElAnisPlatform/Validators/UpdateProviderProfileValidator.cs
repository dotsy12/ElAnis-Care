using ElAnis.Entities.DTO.ServiceProviderProfile;
using FluentValidation;

namespace ElAnis.API.Validators
{
    public class UpdateProviderProfileValidator : AbstractValidator<UpdateProviderProfileRequest>
    {
        public UpdateProviderProfileValidator()
        {
            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .WithMessage("Bio cannot exceed 1000 characters");

            RuleFor(x => x.Experience)
                .MaximumLength(2000)
                .WithMessage("Experience cannot exceed 2000 characters");

            RuleFor(x => x.ProfilePicture)
                .Must(BeValidImage)
                .When(x => x.ProfilePicture != null)
                .WithMessage("Only JPG, JPEG, PNG files are allowed (Max 5MB)");
        }

        private bool BeValidImage(IFormFile? file)
        {
            if (file == null) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024;
        }
    }
}
