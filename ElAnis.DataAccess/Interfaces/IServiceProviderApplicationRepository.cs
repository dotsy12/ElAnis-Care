

using ElAnis.Entities.Models;
namespace ElAnis.DataAccess.Repositories.Interfaces
{
    public interface IServiceProviderApplicationRepository : IGenericRepository<ServiceProviderApplication>
    {
        Task<(IEnumerable<ServiceProviderApplication> Items, int TotalCount)> GetApplicationsWithDetailsAsync(
            int page, int pageSize);
        Task<ServiceProviderApplication?> GetApplicationWithDetailsAsync(Guid id);
        Task<IEnumerable<ServiceProviderApplication>> GetPendingApplicationsAsync();
    }
}