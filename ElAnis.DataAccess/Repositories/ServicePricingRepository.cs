// ===== ServicePricingRepository.cs =====
using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
    public class ServicePricingRepository : GenericRepository<ServicePricing>, IServicePricingRepository
    {
        public ServicePricingRepository(AuthContext context) : base(context) { }

        public async Task<IEnumerable<ServicePricing>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(sp => sp.Category)
                .Where(sp => sp.CategoryId == categoryId)
                .OrderBy(sp => sp.ShiftType)
                .ToListAsync();
        }

        public async Task<ServicePricing?> GetByShiftTypeAsync(Guid categoryId, ShiftType shiftType)
        {
            return await _dbSet
                .Include(sp => sp.Category)
                .FirstOrDefaultAsync(sp => sp.CategoryId == categoryId && sp.ShiftType == shiftType);
        }

        public async Task<bool> ExistsForShiftTypeAsync(Guid categoryId, ShiftType shiftType)
        {
            return await _dbSet
                .AnyAsync(sp => sp.CategoryId == categoryId && sp.ShiftType == shiftType);
        }

        public async Task<IEnumerable<ServicePricing>> GetActivePricingAsync()
        {
            return await _dbSet
                .Include(sp => sp.Category)
                .Where(sp => sp.IsActive && sp.Category.IsActive)
                .OrderBy(sp => sp.Category.Name)
                .ThenBy(sp => sp.ShiftType)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, List<ServicePricing>>> GetAllCategoriesWithPricingAsync()
        {
            var pricings = await _dbSet
                .Include(sp => sp.Category)
                .Where(sp => sp.IsActive && sp.Category.IsActive)
                .OrderBy(sp => sp.ShiftType)
                .ToListAsync();

            return pricings
                .GroupBy(sp => sp.CategoryId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}