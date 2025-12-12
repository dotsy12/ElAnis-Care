using ElAnis.Entities.DTO.Review;
using ElAnis.Entities.Shared.Bases;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Review
{
    public interface IReviewService
    {
        Task<Response<ReviewResponse>> CreateReviewAsync(CreateReviewDto request, ClaimsPrincipal userClaims);
        Task<Response<ProviderReviewsResponse>> GetProviderReviewsAsync(Guid providerId);
        Task<Response<List<ReviewResponse>>> GetUserReviewsAsync(ClaimsPrincipal userClaims);
        Task<Response<ReviewResponse>> GetReviewByRequestIdAsync(Guid serviceRequestId);
    }
}