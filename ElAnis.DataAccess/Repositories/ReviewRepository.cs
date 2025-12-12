using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AuthContext context) : base(context) { }

        public async Task<Review?> GetByServiceRequestIdAsync(Guid serviceRequestId)
        {
            return await _dbSet
                .Include(r => r.Client)
                .Include(r => r.ServiceProvider)
                .Include(r => r.ServiceRequest)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == serviceRequestId);
        }

        public async Task<IEnumerable<Review>> GetProviderReviewsAsync(Guid providerId)
        {
            // نجيب الـ User ID من الـ ServiceProviderProfile
            var provider = await _context.ServiceProviderProfiles
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
                return new List<Review>();

            return await _dbSet
                .Include(r => r.Client)
                .Include(r => r.ServiceRequest)
                .Where(r => r.ServiceProviderUserId == provider.UserId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
        {
            return await _dbSet
                .Include(r => r.ServiceProvider)
                .Include(r => r.ServiceRequest)
                .Where(r => r.ClientUserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetProviderAverageRatingAsync(Guid providerId)
        {
            var provider = await _context.ServiceProviderProfiles
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
                return 0;

            var reviews = await _dbSet
                .Where(r => r.ServiceProviderUserId == provider.UserId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> HasUserReviewedRequestAsync(string userId, Guid serviceRequestId)
        {
            return await _dbSet.AnyAsync(r =>
                r.ClientUserId == userId &&
                r.ServiceRequestId == serviceRequestId);
        }
    }
}