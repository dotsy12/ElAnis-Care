using ElAnis.Entities.DTO.ServiceRequest;
using ElAnis.Utilities.Enum;
using FluentValidation;

namespace ElAnis.API.Validators
{
    public class ProviderResponseValidator : AbstractValidator<ProviderResponseDto>
    {
        public ProviderResponseValidator()
        {
            RuleFor(x => x.Status)
                .Must(s => s == ServiceRequestStatus.Accepted || s == ServiceRequestStatus.Rejected)
                .WithMessage("Status must be either Accepted or Rejected.");

            RuleFor(x => x.Reason)
                .NotEmpty().When(x => x.Status == ServiceRequestStatus.Rejected)
                .WithMessage("Reason is required when rejecting a request.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        }
    }
}