// RejectApplicationRequestValidator.cs
using ElAnis.Entities.DTO.Admin;
using FluentValidation;

namespace ElAnis.API.Validators
{
	public class RejectApplicationRequestValidator : AbstractValidator<RejectApplicationRequest>
	{
		public RejectApplicationRequestValidator()
		{
			RuleFor(x => x.RejectionReason)
				.NotEmpty().WithMessage("Rejection reason is required.")
				.MinimumLength(10).WithMessage("Rejection reason must be at least 10 characters.")
				.MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters.");
		}
	}
}