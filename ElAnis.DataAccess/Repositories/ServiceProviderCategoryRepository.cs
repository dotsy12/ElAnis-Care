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
    public class ServiceProviderCategoryRepository : GenericRepository<ServiceProviderCategory>, IServiceProviderCategoryRepository
    {
        public ServiceProviderCategoryRepository(AuthContext context) : base(context) { }

        public async Task<IEnumerable<ServiceProviderCategory>> GetByServiceProviderIdAsync(Guid serviceProviderId)
        {
            return await _dbSet
                .Where(spc => spc.ServiceProviderId == serviceProviderId)
                .Include(spc => spc.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceProviderCategory>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _dbSet
                .Where(spc => spc.CategoryId == categoryId)
                .Include(spc => spc.ServiceProvider)
                .ToListAsync();
        }
    }
}
