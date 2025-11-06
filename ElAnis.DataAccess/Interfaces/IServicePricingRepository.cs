using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;


namespace ElAnis.DataAccess.Interfaces
{
    public interface IServicePricingRepository : IGenericRepository<ServicePricing>
    {
        Task<IEnumerable<ServicePricing>> GetByCategoryIdAsync(Guid categoryId);
        Task<ServicePricing?> GetByShiftTypeAsync(Guid categoryId, ShiftType shiftType);
        Task<bool> ExistsForShiftTypeAsync(Guid categoryId, ShiftType shiftType);
        Task<IEnumerable<ServicePricing>> GetActivePricingAsync();
        Task<Dictionary<Guid, List<ServicePricing>>> GetAllCategoriesWithPricingAsync();
    }
}
