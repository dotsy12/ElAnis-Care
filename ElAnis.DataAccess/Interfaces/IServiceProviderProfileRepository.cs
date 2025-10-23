using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
    public interface IServiceProviderProfileRepository : IGenericRepository<ServiceProviderProfile>
    {
        Task<ServiceProviderProfile?> GetByUserIdAsync(string userId);
        Task<(IEnumerable<ServiceProviderProfile>, int)> GetProvidersWithDetailsAsync(int page, int pageSize);
    }
}
