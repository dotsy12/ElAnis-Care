using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
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
    }
}
