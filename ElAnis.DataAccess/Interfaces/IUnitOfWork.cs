using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Interfaces;

namespace ElAnis.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository properties
        IUserRepository Users { get; }
        IServiceProviderApplicationRepository ServiceProviderApplications { get; }
        IServiceProviderProfileRepository ServiceProviderProfiles { get; }
        ICategoryRepository Categories { get; }
        IServiceProviderCategoryRepository ServiceProviderCategories { get; }
        IProviderWorkingAreaRepository ProviderWorkingAreas { get; }
        IProviderAvailabilityRepository ProviderAvailabilities { get; }

        // Generic repository for other entities
        IGenericRepository<T> Repository<T>() where T : class;

        // Transaction management
        Task<int> CompleteAsync();
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        // Dispose methods
        void Dispose();
        ValueTask DisposeAsync();
    }
}