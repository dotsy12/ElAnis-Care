using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{

    public class ProviderWorkingAreaRepository : GenericRepository<ProviderWorkingArea>, IProviderWorkingAreaRepository
    {
        public ProviderWorkingAreaRepository(AuthContext context) : base(context) { }

        public async Task<List<ProviderWorkingArea>> GetProviderWorkingAreasAsync(Guid serviceProviderId)
        {
            return await _dbSet
                .Where(w => w.ServiceProviderId == serviceProviderId && w.IsActive)
            .OrderBy(w => w.Governorate)
                .ToListAsync();
        }

        public async Task<bool> IsGovernorateExistsAsync(Guid serviceProviderId, string governorate)
        {
            return await _dbSet
                .AnyAsync(w => w.ServiceProviderId == serviceProviderId
                            && w.Governorate == governorate
                            && w.IsActive);
        }
    }
}
