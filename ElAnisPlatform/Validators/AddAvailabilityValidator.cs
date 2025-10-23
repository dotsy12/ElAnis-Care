using ElAnis.Entities.DTO.Availability;
using FluentValidation;

namespace ElAnis.API.Validators
{
    public class AddAvailabilityValidator : AbstractValidator<AddAvailabilityRequest>
    {
        public AddAvailabilityValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Date cannot be in the past");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}
