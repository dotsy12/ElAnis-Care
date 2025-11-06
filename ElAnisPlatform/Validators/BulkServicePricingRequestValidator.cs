using ElAnis.Entities.DTO.ServicePricing;
using FluentValidation;

namespace ElAnis.API.Validators
{

    // ===== 3. BulkServicePricingRequestValidator.cs =====
    public class BulkServicePricingRequestValidator : AbstractValidator<BulkServicePricingRequest>
    {
        public BulkServicePricingRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.");

            RuleFor(x => x.Pricings)
                .NotEmpty().WithMessage("At least one pricing item is required.")
                .Must(p => p.Count <= 10).WithMessage("Cannot add more than 10 pricing items at once.");

            RuleForEach(x => x.Pricings).ChildRules(pricing =>
            {
                pricing.RuleFor(p => p.ShiftType)
                    .IsInEnum().WithMessage("Invalid shift type.");

                pricing.RuleFor(p => p.PricePerShift)
                    .GreaterThan(0).WithMessage("Price must be greater than zero.")
                    .LessThanOrEqualTo(10000).WithMessage("Price must not exceed 10,000.");

                pricing.RuleFor(p => p.Description)
                    .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
            });
        }
    }
}
