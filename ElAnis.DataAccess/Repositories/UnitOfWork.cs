
// 2. Unit of Work Implementation
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using System.Collections.Concurrent;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthContext _context;
        private IDbContextTransaction? _transaction;
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private bool _disposed = false;

        // Repository properties
        private IUserRepository? _users;
        private IServiceProviderApplicationRepository? _serviceProviderApplications;
        private IServiceProviderProfileRepository? _serviceProviderProfiles;
        private ICategoryRepository? _categories;
        private IServiceProviderCategoryRepository? _serviceProviderCategories;
        private IProviderWorkingAreaRepository? _providerWorkingAreas;
        private IProviderAvailabilityRepository? _providerAvailabilities;
        public UnitOfWork(AuthContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IServiceProviderApplicationRepository ServiceProviderApplications =>
            _serviceProviderApplications ??= new ServiceProviderApplicationRepository(_context);

        public IServiceProviderProfileRepository ServiceProviderProfiles =>
            _serviceProviderProfiles ??= new ServiceProviderProfileRepository(_context);

        public ICategoryRepository Categories =>
            _categories ??= new CategoryRepository(_context);

        public IServiceProviderCategoryRepository ServiceProviderCategories =>
            _serviceProviderCategories ??= new ServiceProviderCategoryRepository(_context);

        public IProviderWorkingAreaRepository ProviderWorkingAreas =>
            _providerWorkingAreas ??= new ProviderWorkingAreaRepository(_context);

        public IProviderAvailabilityRepository ProviderAvailabilities =>
            _providerAvailabilities ??= new ProviderAvailabilityRepository(_context);

        public IGenericRepository<T> Repository<T>() where T : class
        {
            return (IGenericRepository<T>)_repositories.GetOrAdd(typeof(T),
                _ => new GenericRepository<T>(_context));
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }

            await _context.DisposeAsync();
        }
    }
}
