using ElAnis.Entities.DTO.ServiceRequest;
using FluentValidation;

namespace ElAnis.API.Validators
{
    public class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequestDto>
    {
        public CreateServiceRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.ShiftType)
                .IsInEnum().WithMessage("Invalid shift type.");

            RuleFor(x => x.PreferredDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Preferred date must be in the future.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");

            RuleFor(x => x.Governorate)
                .NotEmpty().WithMessage("Governorate is required.")
                .MaximumLength(100).WithMessage("Governorate must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        }
    }
}