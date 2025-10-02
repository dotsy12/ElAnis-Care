using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;


namespace ElAnis.DataAccess.Interfaces
{
    public interface IServiceProviderCategoryRepository : IGenericRepository<ServiceProviderCategory>
    {
        Task<IEnumerable<ServiceProviderCategory>> GetByServiceProviderIdAsync(Guid serviceProviderId);
        Task<IEnumerable<ServiceProviderCategory>> GetByCategoryIdAsync(Guid categoryId);
    }
}
