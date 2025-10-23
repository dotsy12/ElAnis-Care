using ElAnis.Entities.DTO.Availability;
using FluentValidation;

namespace ElAnis.API.Validators
{

    public class UpdateAvailabilityValidator : AbstractValidator<UpdateAvailabilityRequest>
    {
        public UpdateAvailabilityValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Availability ID is required");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
