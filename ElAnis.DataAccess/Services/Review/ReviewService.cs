using ElAnis.Entities.DTO.Review;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewService> _logger;
        private readonly ResponseHandler _responseHandler;

        public ReviewService(
            IUnitOfWork unitOfWork,
            ILogger<ReviewService> logger,
            ResponseHandler responseHandler)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _responseHandler = responseHandler;
        }

        public async Task<Response<ReviewResponse>> CreateReviewAsync(
            CreateReviewDto request,
            ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<ReviewResponse>("User not authenticated");

                // ✅ جلب الـ ServiceRequest
                var serviceRequest = await _unitOfWork.ServiceRequests
                    .GetRequestWithDetailsAsync(request.ServiceRequestId);

                if (serviceRequest == null)
                    return _responseHandler.NotFound<ReviewResponse>("Service request not found");

                // ✅ التحقق من أن المستخدم هو صاحب الطلب
                if (serviceRequest.UserId != userId)
                    return _responseHandler.Forbidden<ReviewResponse>("You can only review your own requests");

                // ✅ التحقق من أن الطلب مكتمل
                if (serviceRequest.Status != ServiceRequestStatus.Completed)
                    return _responseHandler.BadRequest<ReviewResponse>("You can only review completed services");

                // ✅ التحقق من عدم وجود تقييم سابق
                var existingReview = await _unitOfWork.Reviews
                    .HasUserReviewedRequestAsync(userId, request.ServiceRequestId);

                if (existingReview)
                    return _responseHandler.BadRequest<ReviewResponse>("You have already reviewed this service");

                // ✅ جلب الـ Provider
                if (!serviceRequest.ServiceProviderId.HasValue)
                    return _responseHandler.BadRequest<ReviewResponse>("Service provider not found");

                var provider = await _unitOfWork.ServiceProviderProfiles
                    .GetByIdAsync(serviceRequest.ServiceProviderId.Value);

                if (provider == null)
                    return _responseHandler.NotFound<ReviewResponse>("Provider not found");

                // ✅ إنشاء الـ Review
                var review = new Entities.Models.Review
                {
                    ServiceRequestId = request.ServiceRequestId,
                    ClientUserId = userId,
                    ServiceProviderUserId = provider.UserId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Reviews.AddAsync(review);

                // ✅ تحديث إحصائيات الـ Provider
                var allProviderReviews = await _unitOfWork.Reviews.GetProviderReviewsAsync(provider.Id);
                var totalReviews = allProviderReviews.Count() + 1;
                var totalRating = allProviderReviews.Sum(r => r.Rating) + request.Rating;

                provider.AverageRating = (double)totalRating / totalReviews;
                provider.TotalReviews = totalReviews;

                _unitOfWork.ServiceProviderProfiles.Update(provider);

                await _unitOfWork.CompleteAsync();

                // ✅ إرجاع الـ Response
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var response = new ReviewResponse
                {
                    Id = review.Id,
                    ServiceRequestId = review.ServiceRequestId,
                    ClientName = $"{user?.FirstName} {user?.LastName}",
                    ClientAvatar = user?.ProfilePicture ?? "",
                    ProviderName = $"{provider.User.FirstName} {provider.User.LastName}",
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                };

                return _responseHandler.Created(response, "Review submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return _responseHandler.ServerError<ReviewResponse>("Error creating review");
            }
        }

        public async Task<Response<ProviderReviewsResponse>> GetProviderReviewsAsync(Guid providerId)
        {
            try
            {
                var provider = await _unitOfWork.ServiceProviderProfiles.GetByIdAsync(providerId);
                if (provider == null)
                    return _responseHandler.NotFound<ProviderReviewsResponse>("Provider not found");

                var reviews = await _unitOfWork.Reviews.GetProviderReviewsAsync(providerId);
                var averageRating = await _unitOfWork.Reviews.GetProviderAverageRatingAsync(providerId);

                var response = new ProviderReviewsResponse
                {
                    ProviderId = providerId,
                    ProviderName = $"{provider.User.FirstName} {provider.User.LastName}",
                    AverageRating = averageRating,
                    TotalReviews = reviews.Count(),
                    Reviews = reviews.Select(r => new ReviewResponse
                    {
                        Id = r.Id,
                        ServiceRequestId = r.ServiceRequestId,
                        ClientName = $"{r.Client.FirstName} {r.Client.LastName}",
                        ClientAvatar = r.Client.ProfilePicture ?? "",
                        ProviderName = $"{provider.User.FirstName} {provider.User.LastName}",
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    }).ToList()
                };

                return _responseHandler.Success(response, "Reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provider reviews");
                return _responseHandler.ServerError<ProviderReviewsResponse>("Error retrieving reviews");
            }
        }

        public async Task<Response<List<ReviewResponse>>> GetUserReviewsAsync(ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<List<ReviewResponse>>("User not authenticated");

                var reviews = await _unitOfWork.Reviews.GetUserReviewsAsync(userId);

                var responses = reviews.Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    ServiceRequestId = r.ServiceRequestId,
                    ClientName = $"{r.Client.FirstName} {r.Client.LastName}",
                    ClientAvatar = r.Client.ProfilePicture ?? "",
                    ProviderName = $"{r.ServiceProvider.FirstName} {r.ServiceProvider.LastName}",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList();

                return _responseHandler.Success(responses, "User reviews retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user reviews");
                return _responseHandler.ServerError<List<ReviewResponse>>("Error retrieving reviews");
            }
        }

        public async Task<Response<ReviewResponse>> GetReviewByRequestIdAsync(Guid serviceRequestId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByServiceRequestIdAsync(serviceRequestId);
                if (review == null)
                    return _responseHandler.NotFound<ReviewResponse>("Review not found");

                var response = new ReviewResponse
                {
                    Id = review.Id,
                    ServiceRequestId = review.ServiceRequestId,
                    ClientName = $"{review.Client.FirstName} {review.Client.LastName}",
                    ClientAvatar = review.Client.ProfilePicture ?? "",
                    ProviderName = $"{review.ServiceProvider.FirstName} {review.ServiceProvider.LastName}",
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                };

                return _responseHandler.Success(response, "Review retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review");
                return _responseHandler.ServerError<ReviewResponse>("Error retrieving review");
            }
        }
    }
}