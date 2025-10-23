using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;


namespace ElAnis.DataAccess.Interfaces
{
    public interface IProviderWorkingAreaRepository : IGenericRepository<ProviderWorkingArea>
    {
        Task<List<ProviderWorkingArea>> GetProviderWorkingAreasAsync(Guid serviceProviderId);
        Task<bool> IsGovernorateExistsAsync(Guid serviceProviderId, string governorate);
    }
}
