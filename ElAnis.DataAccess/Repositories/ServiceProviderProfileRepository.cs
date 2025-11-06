using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.Repositories
{
    public class ServiceProviderProfileRepository : GenericRepository<ServiceProviderProfile>, IServiceProviderProfileRepository
    {
        public ServiceProviderProfileRepository(AuthContext context) : base(context) { }

        public async Task<ServiceProviderProfile?> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(sp => sp.User)
                .Include(sp => sp.Categories)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);
        }

        public async Task<(IEnumerable<ServiceProviderProfile>, int)> GetProvidersWithDetailsAsync(int page, int pageSize)
        {
            var query = _dbSet
                .Include(sp => sp.User)
                .Include(sp => sp.Categories)
                .OrderByDescending(sp => sp.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }


       
        public async Task<(IEnumerable<ServiceProviderProfile> Items, int TotalCount)> SearchProvidersAsync(
            bool? available,
            string? governorate,
            string? city,
            Guid? categoryId,
            string? searchTerm,
            int page,
            int pageSize)
        {
            var query = _dbSet
                .Include(p => p.User)
                .Include(p => p.Categories).ThenInclude(c => c.Category)
                .Include(p => p.WorkingAreas)
                .Where(p => p.Status == ServiceProviderStatus.Approved);

            if (available.HasValue && available.Value)
                query = query.Where(p => p.IsAvailable);

            if (!string.IsNullOrWhiteSpace(governorate))
                query = query.Where(p => p.WorkingAreas.Any(w => w.Governorate == governorate && w.IsActive));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(p => p.WorkingAreas.Any(w => w.City == city && w.IsActive));

            if (categoryId.HasValue)
                query = query.Where(p => p.Categories.Any(c => c.CategoryId == categoryId.Value));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    (p.User.FirstName + " " + p.User.LastName).Contains(searchTerm) ||
                    p.Bio.Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.IsAvailable)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ServiceProviderProfile?> GetProviderWithDetailsAsync(Guid providerId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Categories).ThenInclude(c => c.Category).ThenInclude(cat => cat.Pricing)
                .Include(p => p.WorkingAreas.Where(w => w.IsActive))
                .Include(p => p.Availability.Where(a => a.Date >= DateTime.UtcNow))
                .FirstOrDefaultAsync(p => p.Id == providerId && p.Status == ServiceProviderStatus.Approved);
        }

        public async Task<List<ServiceProviderProfile>> GetProvidersByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Categories).ThenInclude(c => c.Category)
                .Where(p => p.Status == ServiceProviderStatus.Approved
                    && p.IsAvailable
                    && p.Categories.Any(c => c.CategoryId == categoryId))
                .OrderByDescending(p => p.AverageRating)
                .ToListAsync();
        }

        public async Task<bool> IsProviderAvailableOnDateAsync(Guid providerId, DateTime date, ShiftType shift)
        {
            var availability = await _context.Set<ProviderAvailability>()
                .FirstOrDefaultAsync(a =>
                    a.ServiceProviderId == providerId
                    && a.Date.Date == date.Date
                    && a.IsAvailable
                    && a.AvailableShift == shift);

            return availability != null;
        }
    }
}
