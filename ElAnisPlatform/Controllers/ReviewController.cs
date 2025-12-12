using ElAnis.DataAccess.Services.Review;
using ElAnis.Entities.DTO.Review;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Controller for managing service reviews and ratings
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<CreateReviewDto> _createValidator;

        public ReviewsController(
            IReviewService reviewService,
            ResponseHandler responseHandler,
            IValidator<CreateReviewDto> createValidator)
        {
            _reviewService = reviewService;
            _responseHandler = responseHandler;
            _createValidator = createValidator;
        }

        /// <summary>
        /// Create a review for a completed service (User only)
        /// </summary>
        /// <param name="request">Review creation data</param>
        /// <returns>Created review</returns>
        /// <response code="201">Review created successfully</response>
        /// <response code="400">Invalid request or service not completed</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Can only review your own requests</response>
        /// <response code="404">Service request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Response<ReviewResponse>), 201)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto request)
        {
            if (request == null)
                return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

            ValidationResult validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var response = await _reviewService.CreateReviewAsync(request, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all reviews for a specific provider (Public)
        /// </summary>
        /// <param name="providerId">Provider ID</param>
        /// <returns>Provider reviews with average rating</returns>
        /// <response code="200">Reviews retrieved successfully</response>
        /// <response code="400">Invalid provider ID</response>
        /// <response code="404">Provider not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("provider/{providerId}")]
        [ProducesResponseType(typeof(Response<ProviderReviewsResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetProviderReviews(Guid providerId)
        {
            if (providerId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid provider ID"));

            var response = await _reviewService.GetProviderReviewsAsync(providerId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all reviews created by the authenticated user
        /// </summary>
        /// <returns>List of user's reviews</returns>
        /// <response code="200">Reviews retrieved successfully</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("user")]
        [Authorize]
        [ProducesResponseType(typeof(Response<List<ReviewResponse>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetUserReviews()
        {
            var response = await _reviewService.GetUserReviewsAsync(User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get review for a specific service request
        /// </summary>
        /// <param name="requestId">Service request ID</param>
        /// <returns>Review details</returns>
        /// <response code="200">Review retrieved successfully</response>
        /// <response code="400">Invalid request ID</response>
        /// <response code="404">Review not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("request/{requestId}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<ReviewResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetReviewByRequestId(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

            var response = await _reviewService.GetReviewByRequestIdAsync(requestId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}