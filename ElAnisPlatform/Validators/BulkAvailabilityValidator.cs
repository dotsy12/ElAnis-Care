using ElAnis.Entities.DTO.Availability;
using FluentValidation;

namespace ElAnis.API.Validators
{
    public class BulkAvailabilityValidator : AbstractValidator<BulkAvailabilityRequest>
    {
        public BulkAvailabilityValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after start date");

            RuleFor(x => x.EndDate)
                .Must((request, endDate) => (endDate - request.StartDate).TotalDays <= 90)
                .WithMessage("Date range cannot exceed 90 days");
        }
    }
}
