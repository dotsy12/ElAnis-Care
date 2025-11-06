using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepository
    {
        public ServiceRequestRepository(AuthContext context) : base(context) { }

        public async Task<IEnumerable<ServiceRequest>> GetUserRequestsAsync(string userId)
        {
            return await _dbSet
                .Include(r => r.ServiceProvider).ThenInclude(sp => sp.User)
                .Include(r => r.Category)
                .Include(r => r.Payment)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetProviderRequestsAsync(Guid providerId, ServiceRequestStatus? status = null)
        {
            var query = _dbSet
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Payment)
                .Where(r => r.ServiceProviderId == providerId);

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetRequestWithDetailsAsync(Guid requestId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.ServiceProvider).ThenInclude(sp => sp.User)
                .Include(r => r.Category)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        public async Task<bool> HasPendingRequestAsync(string userId, Guid providerId, DateTime preferredDate)
        {
            return await _dbSet.AnyAsync(r =>
                r.UserId == userId
                && r.ServiceProviderId == providerId
                && r.PreferredDate.Date == preferredDate.Date
                && r.Status == ServiceRequestStatus.Pending);
        }
    }
}