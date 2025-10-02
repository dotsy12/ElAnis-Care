using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace ElAnis.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AuthContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
            .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> HasServiceProvidersAsync(Guid categoryId)
        {
            return await _context.ServiceProviderCategories
                .AnyAsync(spc => spc.CategoryId == categoryId);
        }
    }
}
