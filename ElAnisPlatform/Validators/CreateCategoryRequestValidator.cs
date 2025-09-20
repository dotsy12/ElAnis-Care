using ElAnis.Entities.DTO.Category;
using FluentValidation;

namespace ElAnis.Entities.Validators.Category
{
	public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
	{
		public CreateCategoryRequestValidator()
		{
			// Name - required and must be between 3 and 50 characters
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Category name is required.")
				.MinimumLength(3).WithMessage("Category name must be at least 3 characters long.")
				.MaximumLength(50).WithMessage("Category name must not exceed 50 characters.");

			// NameEn - required and must be between 3 and 50 characters
			RuleFor(x => x.NameEn)
				.NotEmpty().WithMessage("English name is required.")
				.MinimumLength(3).WithMessage("English name must be at least 3 characters long.")
				.MaximumLength(50).WithMessage("English name must not exceed 50 characters.");

			// Description - optional, but if provided must not exceed 250 characters
			RuleFor(x => x.Description)
				.MaximumLength(250).WithMessage("Description must not exceed 250 characters.");

			// Icon - required
			RuleFor(x => x.Icon)
				.NotEmpty().WithMessage("Icon is required.");

			// IsActive - optional, but we validate if null
			RuleFor(x => x.IsActive)
				.NotNull().WithMessage("IsActive field is required.");
		}
	}
}
