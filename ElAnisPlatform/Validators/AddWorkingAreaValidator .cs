using ElAnis.Entities.DTO.WorkingArea;
using FluentValidation;

namespace ElAnis.API.Validators
{
    public class AddWorkingAreaValidator : AbstractValidator<AddWorkingAreaRequest>
    {
        public AddWorkingAreaValidator()
        {
            RuleFor(x => x.Governorate)
                .NotEmpty().WithMessage("Governorate is required")
                .MaximumLength(100).WithMessage("Governorate cannot exceed 100 characters");

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.City));

            RuleFor(x => x.District)
                .MaximumLength(100).WithMessage("District cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.District));
        }
    }

}
