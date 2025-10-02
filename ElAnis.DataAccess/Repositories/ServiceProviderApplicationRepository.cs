using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.Repositories
{
    public class ServiceProviderApplicationRepository : GenericRepository<ServiceProviderApplication>, IServiceProviderApplicationRepository
    {
        public ServiceProviderApplicationRepository(AuthContext context) : base(context) { }

        public async Task<(IEnumerable<ServiceProviderApplication> Items, int TotalCount)> GetApplicationsWithDetailsAsync(
            int page, int pageSize)
        {
            var query = _dbSet
                .Include(a => a.User)
                .Include(a => a.ReviewedBy)
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
            .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ServiceProviderApplication?> GetApplicationWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.ReviewedBy)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ServiceProviderApplication>> GetPendingApplicationsAsync()
        {
            return await _dbSet
                .Where(a => a.Status == ElAnis.Utilities.Enum.ServiceProviderApplicationStatus.Pending)
                .ToListAsync();
        }
    }
}
