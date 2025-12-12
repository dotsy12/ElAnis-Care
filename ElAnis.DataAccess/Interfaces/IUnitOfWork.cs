using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

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
        IServicePricingRepository ServicePricings { get; }
        IServiceRequestRepository ServiceRequests { get; }
        IChatRepository Chats { get; }
        IChatMessageRepository ChatMessages { get; }
        IGenericRepository<UserConnection> UserConnections { get; }


        IReviewRepository Reviews { get; }
        IPaymentRepository Payments { get; }

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