// SuspendServiceProviderRequestValidator.cs
using ElAnis.Entities.DTO.Admin;
using FluentValidation;

namespace ElAnis.API.Validators
{
	public class SuspendServiceProviderRequestValidator : AbstractValidator<SuspendServiceProviderRequest>
	{
		public SuspendServiceProviderRequestValidator()
		{
			RuleFor(x => x.Reason)
				.NotEmpty().WithMessage("Suspension reason is required.")
				.MinimumLength(10).WithMessage("Suspension reason must be at least 10 characters.")
				.MaximumLength(500).WithMessage("Suspension reason cannot exceed 500 characters.");
		}
	}
}
