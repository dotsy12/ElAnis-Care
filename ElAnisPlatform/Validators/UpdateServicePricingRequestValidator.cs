using ElAnis.Entities.DTO.ServicePricing;
using FluentValidation;

namespace ElAnis.API.Validators
{
    // ===== 2. UpdateServicePricingRequestValidator.cs =====
    public class UpdateServicePricingRequestValidator : AbstractValidator<UpdateServicePricingRequest>
    {
        public UpdateServicePricingRequestValidator()
        {
            RuleFor(x => x.PricePerShift)
                .GreaterThan(0).WithMessage("Price must be greater than zero.")
                .LessThanOrEqualTo(10000).WithMessage("Price must not exceed 10,000.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }

}
