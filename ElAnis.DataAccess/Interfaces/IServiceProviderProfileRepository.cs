using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;

namespace ElAnis.DataAccess.Interfaces
{
    public interface IServiceProviderProfileRepository : IGenericRepository<ServiceProviderProfile>
    {
        Task<ServiceProviderProfile?> GetByUserIdAsync(string userId);
        Task<(IEnumerable<ServiceProviderProfile>, int)> GetProvidersWithDetailsAsync(int page, int pageSize);
        Task<ServiceProviderProfile?> GetProviderWithDetailsAsync(Guid providerId);
        Task<List<ServiceProviderProfile>> GetProvidersByCategoryAsync(Guid categoryId);

        Task<bool> IsProviderAvailableOnDateAsync(Guid providerId, DateTime date, ShiftType shift);
        Task<(IEnumerable<ServiceProviderProfile> Items, int TotalCount)> SearchProvidersAsync(
        bool? available,
        string? governorate,
        string? city,
        Guid? categoryId,
        string? searchTerm,
        int page,
        int pageSize);

    }
}
