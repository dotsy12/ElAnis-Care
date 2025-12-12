using ElAnis.Entities.DTO.Review;
using FluentValidation;

namespace ElAnis.Utilities.Validators.Review
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.ServiceRequestId)
                .NotEmpty().WithMessage("Service request ID is required");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
        }
    }
}