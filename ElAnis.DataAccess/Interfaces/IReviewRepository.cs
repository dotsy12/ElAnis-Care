using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<Review?> GetByServiceRequestIdAsync(Guid serviceRequestId);
        Task<IEnumerable<Review>> GetProviderReviewsAsync(Guid providerId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);
        Task<double> GetProviderAverageRatingAsync(Guid providerId);
        Task<bool> HasUserReviewedRequestAsync(string userId, Guid serviceRequestId);
    }
}