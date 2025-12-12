## ⚠️ NOTE: Filtered & Compact Export
---

# ICategoryRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
public interface ICategoryRepository : IGenericRepository<Category>
{
Task<IEnumerable<Category>> GetActiveCategoriesAsync();
Task<bool> HasServiceProvidersAsync(Guid categoryId);
}
}
```

# IGenericRepository.cs
```cs
﻿using System.Linq.Expressions;

namespace ElAnis.DataAccess.Repositories.Interfaces
{
public interface IGenericRepository<T> where T : class
{
Task<T?> GetByIdAsync(object id);
Task<IEnumerable<T>> GetAllAsync();
Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
Task<T?> FindSingleAsync(Expression<Func<T, bool>> predicate);
Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

IQueryable<T> GetQueryable();

Task<T> AddAsync(T entity);
Task AddRangeAsync(IEnumerable<T> entities);
void Update(T entity);
void UpdateRange(IEnumerable<T> entities);
void Delete(T entity);
void DeleteRange(IEnumerable<T> entities);

Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
int page,
int pageSize,
Expression<Func<T, bool>>? filter = null,
Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
}
}
```

# IPaymentRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Repositories
{
public interface IPaymentRepository : IGenericRepository<Payment>
{
Task<Payment?> GetByServiceRequestIdAsync(Guid serviceRequestId);
Task<Payment?> GetByTransactionIdAsync(string transactionId);
}
}
```

# IProviderAvailabilityRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;

namespace ElAnis.DataAccess.Interfaces
{
public interface IProviderAvailabilityRepository : IGenericRepository<ProviderAvailability>
{
Task<IEnumerable<ProviderAvailability>> GetProviderAvailabilityAsync(Guid providerId, DateTime startDate, DateTime endDate);
Task<ProviderAvailability?> GetByDateAsync(Guid serviceProviderId, DateTime date);
Task<bool> IsAvailableOnDateAsync(Guid serviceProviderId, DateTime date, ShiftType? shiftType = null);
Task<List<DateTime>> GetBookedDatesAsync(Guid serviceProviderId, DateTime startDate, DateTime endDate);
Task<ProviderAvailability?> GetByDateAndShiftAsync(Guid providerId, DateTime date, ShiftType? shift);
}
}
```

# IProviderWorkingAreaRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
public interface IProviderWorkingAreaRepository : IGenericRepository<ProviderWorkingArea>
{
Task<List<ProviderWorkingArea>> GetProviderWorkingAreasAsync(Guid serviceProviderId);
Task<bool> IsGovernorateExistsAsync(Guid serviceProviderId, string governorate);
}
}
```

# IReviewRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
public interface IReviewRepository : IGenericRepository<Review>
{
Task<Review?> GetByServiceRequestIdAsync(Guid serviceRequestId);
Task<IEnumerable<Review>> GetProviderReviewsAsync(Guid providerId);
Task<IEnumerable<Review>> GetUserReviewsAsync(string userId);
Task<double> GetProviderAverageRatingAsync(Guid providerId);
Task<bool> HasUserReviewedRequestAsync(string userId, Guid serviceRequestId);
}
}
```

# IServicePricingRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
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
```

# IServiceProviderApplicationRepository.cs
```cs
﻿

using ElAnis.Entities.Models;
namespace ElAnis.DataAccess.Repositories.Interfaces
{
public interface IServiceProviderApplicationRepository : IGenericRepository<ServiceProviderApplication>
{
Task<(IEnumerable<ServiceProviderApplication> Items, int TotalCount)> GetApplicationsWithDetailsAsync(
int page, int pageSize);
Task<ServiceProviderApplication?> GetApplicationWithDetailsAsync(Guid id);
Task<IEnumerable<ServiceProviderApplication>> GetPendingApplicationsAsync();
Task<ServiceProviderApplication?> GetByUserIdAsync(string userId);

}
}
```

# IServiceProviderCategoryRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Interfaces
{
public interface IServiceProviderCategoryRepository : IGenericRepository<ServiceProviderCategory>
{
Task<IEnumerable<ServiceProviderCategory>> GetByServiceProviderIdAsync(Guid serviceProviderId);
Task<IEnumerable<ServiceProviderCategory>> GetByCategoryIdAsync(Guid categoryId);
}
}
```

# IServiceProviderProfileRepository.cs
```cs
﻿using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;

namespace ElAnis.DataAccess.Interfaces
{
public interface IServiceProviderProfileRepository : IGenericRepository<ServiceProviderProfile>
{
Task<ServiceProviderProfile?> GetByUserIdAsync(string userId);
Task<(IEnumerable<ServiceProviderProfile>, int)> GetProvidersWithDetailsAsync(int page, int pageSize);
Task<ServiceProviderProfile?> GetProviderWithDetailsAsync(Guid providerId);
Task<List<ServiceProviderProfile>> GetProvidersByCategoryAsync(Guid categoryId);

Task<bool> IsProviderAvailableOnDateAsync(Guid providerId, DateTime date, ShiftType shift);
Task<(IEnumerable<ServiceProviderProfile> Items, int TotalCount)> SearchProvidersAsync(
bool? available,
string? governorate,
string? city,
Guid? categoryId,
string? searchTerm,
int page,
int pageSize);

}
}
```

# IServiceRequestRepository.cs
```cs
﻿using ElAnis.Entities.Models;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Utilities.Enum;

namespace ElAnis.DataAccess.Repositories
{
public interface IServiceRequestRepository : IGenericRepository<ServiceRequest>
{
Task<IEnumerable<ServiceRequest>> GetUserRequestsAsync(string userId);
Task<IEnumerable<ServiceRequest>> GetProviderRequestsAsync(Guid providerId, ServiceRequestStatus? status = null);
Task<ServiceRequest?> GetRequestWithDetailsAsync(Guid requestId);
Task<bool> HasPendingRequestAsync(string userId, Guid providerId, DateTime preferredDate);
}
}
```

# IUnitOfWork.cs
```cs
﻿using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories;
using ElAnis.DataAccess.Repositories.Interfaces;

namespace ElAnis.DataAccess
{
public interface IUnitOfWork : IDisposable
{
IUserRepository Users { get; }
IServiceProviderApplicationRepository ServiceProviderApplications { get; }
IServiceProviderProfileRepository ServiceProviderProfiles { get; }
ICategoryRepository Categories { get; }
IServiceProviderCategoryRepository ServiceProviderCategories { get; }
IProviderWorkingAreaRepository ProviderWorkingAreas { get; }
IProviderAvailabilityRepository ProviderAvailabilities { get; }
IServicePricingRepository ServicePricings { get; }
IServiceRequestRepository ServiceRequests { get; }

IReviewRepository Reviews { get; }
IPaymentRepository Payments { get; }

IGenericRepository<T> Repository<T>() where T : class;

Task<int> CompleteAsync();
Task<int> SaveChangesAsync();
Task BeginTransactionAsync();
Task CommitAsync();
Task RollbackAsync();

void Dispose();
ValueTask DisposeAsync();
}
}
```

# IUserRepository.cs
```cs
﻿using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.DataAccess.Repositories.Interfaces
{
public interface IUserRepository : IGenericRepository<User>
{
Task<User?> FindByEmailAsync(string email);
Task<User?> FindByPhoneAsync(string phone);
Task<User?> FindByEmailOrPhoneAsync(string email, string phone);
Task<bool> IsEmailExistsAsync(string email);
Task<bool> IsPhoneExistsAsync(string phone);
}
}
```

# CategoryRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
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
```

# GenericRepository.cs
```cs
﻿// 2. Generic Repository Implementation
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Interfaces;

namespace ElAnis.DataAccess.Repositories.Implementations
{
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
protected readonly AuthContext _context;
protected readonly DbSet<T> _dbSet;

public GenericRepository(AuthContext context)
{
_context = context;
_dbSet = context.Set<T>();
}

public async Task<T?> GetByIdAsync(object id)
{
return await _dbSet.FindAsync(id);
}

public async Task<IEnumerable<T>> GetAllAsync()
{
return await _dbSet.ToListAsync();
}

public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
{
return await _dbSet.Where(predicate).ToListAsync();
}

public async Task<T?> FindSingleAsync(Expression<Func<T, bool>> predicate)
{
return await _dbSet.FirstOrDefaultAsync(predicate);
}

public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
{
return await _dbSet.AnyAsync(predicate);
}

public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
{
return predicate == null
? await _dbSet.CountAsync()
: await _dbSet.CountAsync(predicate);
}

public IQueryable<T> GetQueryable()
{
return _dbSet.AsQueryable();
}

public async Task<T> AddAsync(T entity)
{
await _dbSet.AddAsync(entity);
return entity;
}

public async Task AddRangeAsync(IEnumerable<T> entities)
{
await _dbSet.AddRangeAsync(entities);
}

public void Update(T entity)
{
_dbSet.Update(entity);
}

public void UpdateRange(IEnumerable<T> entities)
{
_dbSet.UpdateRange(entities);
}

public void Delete(T entity)
{
_dbSet.Remove(entity);
}

public void DeleteRange(IEnumerable<T> entities)
{
_dbSet.RemoveRange(entities);
}

public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
int page,
int pageSize,
Expression<Func<T, bool>>? filter = null,
Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
{
var query = _dbSet.AsQueryable();

if (filter != null)
query = query.Where(filter);

var totalCount = await query.CountAsync();

if (orderBy != null)
query = orderBy(query);

var items = await query
.Skip((page - 1) * pageSize)
.Take(pageSize)
.ToListAsync();

return (items, totalCount);
}
}
}
```

# PaymentRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
public PaymentRepository(AuthContext context) : base(context) { }

public async Task<Payment?> GetByServiceRequestIdAsync(Guid serviceRequestId)
{
return await _dbSet
.Include(p => p.ServiceRequest)
.FirstOrDefaultAsync(p => p.ServiceRequestId == serviceRequestId);
}

public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
{
return await _dbSet
.Include(p => p.ServiceRequest)
.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
}
}
}
```

# ProviderAvailabilityRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
public class ProviderAvailabilityRepository : GenericRepository<ProviderAvailability>, IProviderAvailabilityRepository
{
public ProviderAvailabilityRepository(AuthContext context) : base(context) { }

public async Task<IEnumerable<ProviderAvailability>> GetProviderAvailabilityAsync(
Guid providerId,
DateTime startDate,
DateTime endDate)
{
return await _dbSet
.Where(a => a.ServiceProviderId == providerId
&& a.Date.Date >= startDate.Date
&& a.Date.Date <= endDate.Date)
.OrderBy(a => a.Date)
.ToListAsync();
}

public async Task<ProviderAvailability?> GetByDateAsync(Guid providerId, DateTime date)
{
return await _dbSet
.FirstOrDefaultAsync(a => a.ServiceProviderId == providerId
&& a.Date.Date == date.Date);
}
public async Task<bool> IsAvailableOnDateAsync(
Guid serviceProviderId,
DateTime date,
ShiftType? shiftType = null)
{
var targetDate = date.Date;
var availability = await _dbSet
.FirstOrDefaultAsync(a => a.ServiceProviderId == serviceProviderId
&& a.Date.Date == targetDate);

if (availability == null || !availability.IsAvailable)
return false;

if (shiftType.HasValue && availability.AvailableShift.HasValue)
return availability.AvailableShift == shiftType;

return true;
}

public async Task<List<DateTime>> GetBookedDatesAsync(
Guid serviceProviderId,
DateTime startDate,
DateTime endDate)
{
var start = startDate.Date;
var end = endDate.Date;

return await _context.ServiceRequests
.Where(sr => sr.ServiceProviderId == serviceProviderId
&& sr.PreferredDate.Date >= start
&& sr.PreferredDate.Date <= end
&& (sr.Status == ServiceRequestStatus.Accepted
|| sr.Status == ServiceRequestStatus.Paid
|| sr.Status == ServiceRequestStatus.InProgress))
.Select(sr => sr.PreferredDate.Date)
.Distinct()
.ToListAsync();
}
public async Task<ProviderAvailability?> GetByDateAndShiftAsync(
Guid providerId,
DateTime date,
ShiftType? shift)
{
return await _dbSet
.FirstOrDefaultAsync(a =>
a.ServiceProviderId == providerId
&& a.Date.Date == date.Date
&& a.AvailableShift == shift);
}

}
}
```

# ProviderWorkingAreaRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{

public class ProviderWorkingAreaRepository : GenericRepository<ProviderWorkingArea>, IProviderWorkingAreaRepository
{
public ProviderWorkingAreaRepository(AuthContext context) : base(context) { }

public async Task<List<ProviderWorkingArea>> GetProviderWorkingAreasAsync(Guid serviceProviderId)
{
return await _dbSet
.Where(w => w.ServiceProviderId == serviceProviderId && w.IsActive)
.OrderBy(w => w.Governorate)
.ToListAsync();
}

public async Task<bool> IsGovernorateExistsAsync(Guid serviceProviderId, string governorate)
{
return await _dbSet
.AnyAsync(w => w.ServiceProviderId == serviceProviderId
&& w.Governorate == governorate
&& w.IsActive);
}
}
}
```

# ReviewRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
public ReviewRepository(AuthContext context) : base(context) { }

public async Task<Review?> GetByServiceRequestIdAsync(Guid serviceRequestId)
{
return await _dbSet
.Include(r => r.Client)
.Include(r => r.ServiceProvider)
.Include(r => r.ServiceRequest)
.FirstOrDefaultAsync(r => r.ServiceRequestId == serviceRequestId);
}

public async Task<IEnumerable<Review>> GetProviderReviewsAsync(Guid providerId)
{
var provider = await _context.ServiceProviderProfiles
.FirstOrDefaultAsync(p => p.Id == providerId);

if (provider == null)
return new List<Review>();

return await _dbSet
.Include(r => r.Client)
.Include(r => r.ServiceRequest)
.Where(r => r.ServiceProviderUserId == provider.UserId)
.OrderByDescending(r => r.CreatedAt)
.ToListAsync();
}

public async Task<IEnumerable<Review>> GetUserReviewsAsync(string userId)
{
return await _dbSet
.Include(r => r.ServiceProvider)
.Include(r => r.ServiceRequest)
.Where(r => r.ClientUserId == userId)
.OrderByDescending(r => r.CreatedAt)
.ToListAsync();
}

public async Task<double> GetProviderAverageRatingAsync(Guid providerId)
{
var provider = await _context.ServiceProviderProfiles
.FirstOrDefaultAsync(p => p.Id == providerId);

if (provider == null)
return 0;

var reviews = await _dbSet
.Where(r => r.ServiceProviderUserId == provider.UserId)
.ToListAsync();

if (!reviews.Any())
return 0;

return reviews.Average(r => r.Rating);
}

public async Task<bool> HasUserReviewedRequestAsync(string userId, Guid serviceRequestId)
{
return await _dbSet.AnyAsync(r =>
r.ClientUserId == userId &&
r.ServiceRequestId == serviceRequestId);
}
}
}
```

# ServicePricingRepository.cs
```cs
﻿// ===== ServicePricingRepository.cs =====
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
```

# ServiceProviderApplicationRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

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
public async Task<ServiceProviderApplication?> GetByUserIdAsync(string userId)
{
return await _dbSet
.Include(a => a.User)
.FirstOrDefaultAsync(a => a.UserId == userId);
}

}
}
```

# ServiceProviderCategoryRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using Microsoft.EntityFrameworkCore;

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
```

# ServiceProviderProfileRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;

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
```

# ServiceRequestRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepository
{
public ServiceRequestRepository(AuthContext context) : base(context) { }

public async Task<IEnumerable<ServiceRequest>> GetUserRequestsAsync(string userId)
{
return await _dbSet
.Include(r => r.ServiceProvider).ThenInclude(sp => sp.User)
.Include(r => r.Category)
.Include(r => r.Payment)
.Where(r => r.UserId == userId)
.OrderByDescending(r => r.CreatedAt)
.ToListAsync();
}

public async Task<IEnumerable<ServiceRequest>> GetProviderRequestsAsync(Guid providerId, ServiceRequestStatus? status = null)
{
var query = _dbSet
.Include(r => r.User)
.Include(r => r.Category)
.Include(r => r.Payment)
.Where(r => r.ServiceProviderId == providerId);

if (status.HasValue)
query = query.Where(r => r.Status == status.Value);

return await query
.OrderByDescending(r => r.CreatedAt)
.ToListAsync();
}

public async Task<ServiceRequest?> GetRequestWithDetailsAsync(Guid requestId)
{
return await _dbSet
.Include(r => r.User)
.Include(r => r.ServiceProvider).ThenInclude(sp => sp.User)
.Include(r => r.Category)
.Include(r => r.Payment)
.FirstOrDefaultAsync(r => r.Id == requestId);
}

public async Task<bool> HasPendingRequestAsync(string userId, Guid providerId, DateTime preferredDate)
{
return await _dbSet.AnyAsync(r =>
r.UserId == userId
&& r.ServiceProviderId == providerId
&& r.PreferredDate.Date == preferredDate.Date
&& r.Status == ServiceRequestStatus.Pending);
}
}
}
```

# UnitOfWork.cs
```cs
﻿
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

private IUserRepository? _users;
private IServiceProviderApplicationRepository? _serviceProviderApplications;
private IServiceProviderProfileRepository? _serviceProviderProfiles;
private ICategoryRepository? _categories;
private IServiceProviderCategoryRepository? _serviceProviderCategories;
private IProviderWorkingAreaRepository? _providerWorkingAreas;
private IProviderAvailabilityRepository? _providerAvailabilities;
private IServicePricingRepository? _servicePricings;
private IServiceRequestRepository? _serviceRequests;
private IPaymentRepository? _payments;
private IReviewRepository? _reviews;
public UnitOfWork(AuthContext context)
{
_context = context;
_repositories = new ConcurrentDictionary<Type, object>();
}

public IReviewRepository Reviews =>
_reviews ??= new ReviewRepository(_context);

public IPaymentRepository Payments =>
_payments ??= new PaymentRepository(_context);
public IUserRepository Users =>
_users ??= new UserRepository(_context);

public IServiceRequestRepository ServiceRequests =>
_serviceRequests ??= new ServiceRequestRepository(_context);

public IServicePricingRepository ServicePricings =>
_servicePricings ??= new ServicePricingRepository(_context);

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
```

# UserRepository.cs
```cs
﻿using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models.Auth.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElAnis.DataAccess.Repositories
{
public class UserRepository : GenericRepository<User>, IUserRepository
{
public UserRepository(AuthContext context) : base(context) { }

public async Task<User?> FindByEmailAsync(string email)=> await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

public async Task<User?> FindByPhoneAsync(string phone)=> await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phone);

public async Task<User?> FindByEmailOrPhoneAsync(string email, string phone)
{
if (!string.IsNullOrEmpty(email))
return await FindByEmailAsync(email);
if (!string.IsNullOrEmpty(phone))
return await FindByPhoneAsync(phone);
return null;
}

public async Task<bool> IsEmailExistsAsync(string email) => await _dbSet.AnyAsync(u => u.Email == email);

public async Task<bool> IsPhoneExistsAsync(string phone) => await _dbSet.AnyAsync(u => u.PhoneNumber == phone);

}
}
```

# AdminService.cs
```cs
﻿using System.Security.Claims;
using ElAnis.DataAccess.Repositories;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElAnis.DataAccess.Services.Admin
{
public class AdminService : IAdminService
{
private readonly IUnitOfWork _unitOfWork;
private readonly UserManager<User> _userManager;
private readonly ResponseHandler _responseHandler;
private readonly ILogger<AdminService> _logger;

public AdminService(
IUnitOfWork unitOfWork,
UserManager<User> userManager,
ResponseHandler responseHandler,
ILogger<AdminService> logger)
{
_unitOfWork = unitOfWork;
_userManager = userManager;
_responseHandler = responseHandler;
_logger = logger;
}

public async Task<Response<PaginatedResult<ServiceProviderApplicationDto>>> GetServiceProviderApplicationsAsync(int page, int pageSize)
{
try
{
var (applications, totalCount) = await _unitOfWork.ServiceProviderApplications
.GetApplicationsWithDetailsAsync(page, pageSize);

var applicationDtos = applications.Select(a => new ServiceProviderApplicationDto
{
Id = a.Id,
UserId = a.UserId,
UserEmail = a.User.Email ?? "",
FirstName = a.FirstName,
LastName = a.LastName,
PhoneNumber = a.PhoneNumber,
Bio = a.Bio,
Experience = a.Experience,
HourlyRate = a.HourlyRate,
Status = a.Status,
CreatedAt = a.CreatedAt,
ReviewedAt = a.ReviewedAt,
ReviewedByName = a.ReviewedBy != null ? $"{a.ReviewedBy.FirstName} {a.ReviewedBy.LastName}" : null,
RejectionReason = a.RejectionReason
}).ToList();

var result = new PaginatedResult<ServiceProviderApplicationDto>
{
Items = applicationDtos,
TotalCount = totalCount,
Page = page,
PageSize = pageSize
};

return _responseHandler.Success(result, "Applications retrieved successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving service provider applications");
return _responseHandler.ServerError<PaginatedResult<ServiceProviderApplicationDto>>("Error retrieving applications");
}
}

public async Task<Response<ServiceProviderApplicationDetailDto>> GetServiceProviderApplicationByIdAsync(Guid id)
{
try
{
var application = await _unitOfWork.ServiceProviderApplications
.GetApplicationWithDetailsAsync(id);

if (application == null)
return _responseHandler.NotFound<ServiceProviderApplicationDetailDto>("Application not found");

var result = new ServiceProviderApplicationDetailDto
{
Id = application.Id,
UserId = application.UserId,
UserEmail = application.User.Email ?? "",
FirstName = application.FirstName,
LastName = application.LastName,
PhoneNumber = application.PhoneNumber,
Address = application.Address,
DateOfBirth = application.DateOfBirth,
Bio = application.Bio,
NationalId = application.NationalId,
Experience = application.Experience,
HourlyRate = application.HourlyRate,
IdDocumentPath = application.IdDocumentPath,
CertificatePath = application.CertificatePath,
SelectedCategories = application.SelectedCategories,
Status = application.Status,
RejectionReason = application.RejectionReason,
CreatedAt = application.CreatedAt,
ReviewedAt = application.ReviewedAt,
ReviewedByName = application.ReviewedBy != null ? $"{application.ReviewedBy.FirstName} {application.ReviewedBy.LastName}" : null
};

return _responseHandler.Success(result, "Application details retrieved successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving application details for ID: {ApplicationId}", id);
return _responseHandler.ServerError<ServiceProviderApplicationDetailDto>("Error retrieving application details");
}
}
public async Task<Response<string>> ApproveServiceProviderApplicationAsync(
Guid applicationId,
ClaimsPrincipal adminClaims)
{
try
{
var adminId = adminClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(adminId))
return _responseHandler.Unauthorized<string>("Admin not authenticated");

await _unitOfWork.BeginTransactionAsync();

var application = await _unitOfWork.ServiceProviderApplications
.GetApplicationWithDetailsAsync(applicationId);

if (application == null)
{
await _unitOfWork.RollbackAsync();
return _responseHandler.NotFound<string>("Application not found");
}

if (application.Status != ServiceProviderApplicationStatus.Pending)
{
await _unitOfWork.RollbackAsync();
return _responseHandler.BadRequest<string>("Application has already been reviewed");
}

application.Status = ServiceProviderApplicationStatus.Approved;
application.ReviewedAt = DateTime.UtcNow;
application.ReviewedById = adminId;
_unitOfWork.ServiceProviderApplications.Update(application);

var user = application.User;

var serviceProviderProfile = new ServiceProviderProfile
{
UserId = user.Id,
Bio = application.Bio,
NationalId = application.NationalId,
Experience = application.Experience,
HourlyRate = application.HourlyRate,
IdDocumentPath = application.IdDocumentPath,
CertificatePath = application.CertificatePath,
CVPath = application.CVPath,
Status = ServiceProviderStatus.Approved, // 👈 مهم جداً
IsAvailable = true,
ApprovedAt = DateTime.UtcNow
};

await _unitOfWork.ServiceProviderProfiles.AddAsync(serviceProviderProfile);

if (application.SelectedCategories != null && application.SelectedCategories.Count > 0)
{
try
{
var categoryIds = application.SelectedCategories;
var existingCategories = await _unitOfWork.Categories
.FindAsync(c => categoryIds.Contains(c.Id) && c.IsActive);

if (existingCategories.Any())
{
var categoryRelationships = existingCategories.Select(cat => new ServiceProviderCategory
{
ServiceProviderId = serviceProviderProfile.Id,
CategoryId = cat.Id,
CreatedAt = DateTime.UtcNow
}).ToList();

await _unitOfWork.ServiceProviderCategories.AddRangeAsync(categoryRelationships);
}
}
catch (Exception ex)
{
_logger.LogWarning(ex, "Failed to process selected categories for application {ApplicationId}", applicationId);
}
}

await _unitOfWork.CommitAsync();

_logger.LogInformation(
"Service provider application {ApplicationId} approved by admin {AdminId}. " +
"User must LOGIN AGAIN to get updated token with ServiceProviderStatus claim.",
applicationId, adminId);

return _responseHandler.Success<string>(
null,
"Application approved successfully. User must login again to access dashboard.");
}
catch (Exception ex)
{
await _unitOfWork.RollbackAsync();
_logger.LogError(ex, "Error approving application {ApplicationId}", applicationId);
return _responseHandler.ServerError<string>("Error approving application");
}
}
public async Task<Response<string>> RejectServiceProviderApplicationAsync(Guid applicationId, string rejectionReason, ClaimsPrincipal adminClaims)
{
try
{
var adminId = adminClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(adminId))
return _responseHandler.Unauthorized<string>("Admin not authenticated");

var application = await _unitOfWork.ServiceProviderApplications.GetByIdAsync(applicationId);

if (application == null)
return _responseHandler.NotFound<string>("Application not found");

if (application.Status != ServiceProviderApplicationStatus.Pending)
return _responseHandler.BadRequest<string>("Application has already been reviewed");

application.Status = ServiceProviderApplicationStatus.Rejected;
application.RejectionReason = rejectionReason;
application.ReviewedAt = DateTime.UtcNow;
application.ReviewedById = adminId;

_unitOfWork.ServiceProviderApplications.Update(application);
await _unitOfWork.CompleteAsync();

_logger.LogInformation("Service provider application {ApplicationId} rejected by admin {AdminId}", applicationId, adminId);
return _responseHandler.Success<string>(null, "Application rejected successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error rejecting application {ApplicationId}", applicationId);
return _responseHandler.ServerError<string>("Error rejecting application");
}
}

public async Task<Response<AdminDashboardStatsDto>> GetDashboardStatsAsync()
{
try
{
var stats = new AdminDashboardStatsDto();

stats.TotalUsers = await _unitOfWork.Users.CountAsync(u => !u.IsDeleted);
stats.TotalServiceProviders = await _unitOfWork.ServiceProviderProfiles.CountAsync();
stats.PendingApplications = await _unitOfWork.ServiceProviderApplications.CountAsync(
a => a.Status == ServiceProviderApplicationStatus.Pending);

var serviceRequestRepo = _unitOfWork.Repository<ElAnis.Entities.Models.ServiceRequest>();
var reviewRepo = _unitOfWork.Repository<ElAnis.Entities.Models.Review>();

stats.TotalServiceRequests = await serviceRequestRepo.CountAsync();
stats.CompletedServiceRequests = await serviceRequestRepo.CountAsync(
sr => sr.Status == ServiceRequestStatus.Completed);
stats.TotalReviews = await reviewRepo.CountAsync();

var serviceProviders = await _unitOfWork.ServiceProviderProfiles.GetAllAsync();
var providersList = serviceProviders.ToList();

stats.TotalEarnings = providersList.Count > 0 ? providersList.Sum(sp => sp.TotalEarnings) : 0;

var providersWithReviews = providersList.Where(sp => sp.TotalReviews > 0).ToList();
stats.AverageRating = providersWithReviews.Count > 0 ?
providersWithReviews.Average(sp => sp.AverageRating) : 0;

return _responseHandler.Success(stats, "Dashboard stats retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving dashboard stats");
return _responseHandler.ServerError<AdminDashboardStatsDto>("Error retrieving dashboard stats");
}
}

public async Task<Response<PaginatedResult<ServiceProviderDto>>> GetServiceProvidersAsync(int page, int pageSize)
{
try
{
var (serviceProviders, totalCount) = await _unitOfWork.ServiceProviderProfiles
.GetProvidersWithDetailsAsync(page, pageSize);

var serviceProviderDtos = serviceProviders.Select(sp => new ServiceProviderDto
{
Id = sp.Id,
UserId = sp.UserId,
UserEmail = sp.User.Email ?? "",
FirstName = sp.User.FirstName,
LastName = sp.User.LastName,
PhoneNumber = sp.User.PhoneNumber ?? "",
HourlyRate = sp.HourlyRate,
Status = sp.Status,
IsAvailable = sp.IsAvailable,
CompletedJobs = sp.CompletedJobs,
TotalEarnings = sp.TotalEarnings,
AverageRating = sp.AverageRating,
CreatedAt = sp.CreatedAt
}).ToList();

var result = new PaginatedResult<ServiceProviderDto>
{
Items = serviceProviderDtos,
TotalCount = totalCount,
Page = page,
PageSize = pageSize
};

return _responseHandler.Success(result, "Service providers retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving service providers");
return _responseHandler.ServerError<PaginatedResult<ServiceProviderDto>>("Error retrieving service providers");
}
}

public async Task<Response<string>> SuspendServiceProviderAsync(Guid serviceProviderId, string reason, ClaimsPrincipal adminClaims)
{
try
{
var serviceProvider = await _unitOfWork.ServiceProviderProfiles.GetByIdAsync(serviceProviderId);

if (serviceProvider == null)
return _responseHandler.NotFound<string>("Service provider not found");

serviceProvider.Status = ServiceProviderStatus.Suspended;
serviceProvider.RejectionReason = reason;
serviceProvider.IsAvailable = false;

_unitOfWork.ServiceProviderProfiles.Update(serviceProvider);
await _unitOfWork.CompleteAsync();

var adminId = adminClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
_logger.LogInformation("Service provider {ServiceProviderId} suspended by admin {AdminId}", serviceProviderId, adminId);

return _responseHandler.Success<string>(null, "Service provider suspended successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error suspending service provider {ServiceProviderId}", serviceProviderId);
return _responseHandler.ServerError<string>("Error suspending service provider");
}
}

public async Task<Response<string>> ActivateServiceProviderAsync(Guid serviceProviderId, ClaimsPrincipal adminClaims)
{
try
{
var serviceProvider = await _unitOfWork.ServiceProviderProfiles.GetByIdAsync(serviceProviderId);

if (serviceProvider == null)
return _responseHandler.NotFound<string>("Service provider not found");

serviceProvider.Status = ServiceProviderStatus.Approved;
serviceProvider.RejectionReason = null;
serviceProvider.IsAvailable = true;

_unitOfWork.ServiceProviderProfiles.Update(serviceProvider);
await _unitOfWork.CompleteAsync();

var adminId = adminClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
_logger.LogInformation("Service provider {ServiceProviderId} activated by admin {AdminId}", serviceProviderId, adminId);

return _responseHandler.Success<string>(null, "Service provider activated successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error activating service provider {ServiceProviderId}", serviceProviderId);
return _responseHandler.ServerError<string>("Error activating service provider");
}
}

public async Task<Response<PaginatedResult<UserManagementDto>>> GetUsersAsync(GetUsersRequest request)
{
try
{
var query = _userManager.Users.AsQueryable();

if (!string.IsNullOrWhiteSpace(request.Search))
{
var searchLower = request.Search.ToLower();
query = query.Where(u =>
u.FirstName.ToLower().Contains(searchLower) ||
u.LastName.ToLower().Contains(searchLower) ||
u.Email.ToLower().Contains(searchLower) ||
u.PhoneNumber.Contains(searchLower));
}

if (!string.IsNullOrWhiteSpace(request.Status))
{
bool isActive = request.Status.ToLower() == "active";
query = query.Where(u => !u.IsDeleted == isActive);
}

var totalCount = await query.CountAsync();

var users = await query
.OrderByDescending(u => u.CreatedAt)
.Skip((request.Page - 1) * request.PageSize)
.Take(request.PageSize)
.ToListAsync();

var userDtos = new List<UserManagementDto>();

foreach (var user in users)
{
var roles = await _userManager.GetRolesAsync(user);
var role = roles.FirstOrDefault() ?? "user";

userDtos.Add(new UserManagementDto
{
Id = user.Id,
Name = $"{user.FirstName} {user.LastName}",
Email = user.Email ?? "",
Phone = user.PhoneNumber ?? "",
Role = role.ToLower(),
Status = !user.IsDeleted ? "active" : "inactive",
Joined = user.CreatedAt,
ProfilePicture = user.ProfilePicture
});
}

if (!string.IsNullOrWhiteSpace(request.Role))
{
userDtos = userDtos.Where(u => u.Role.ToLower() == request.Role.ToLower()).ToList();
totalCount = userDtos.Count;
}

var result = new PaginatedResult<UserManagementDto>
{
Items = userDtos,
TotalCount = totalCount,
Page = request.Page,
PageSize = request.PageSize
};

return _responseHandler.Success(result, "Users retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving users");
return _responseHandler.ServerError<PaginatedResult<UserManagementDto>>("Error retrieving users");
}
}

public async Task<Response<string>> SuspendUserAsync(string userId)
{
try
{
var user = await _userManager.FindByIdAsync(userId);
if (user == null)
return _responseHandler.NotFound<string>("User not found");

user.IsDeleted = true;
var result = await _userManager.UpdateAsync(user);

if (result.Succeeded)
{
_logger.LogInformation("User {UserId} suspended by admin", userId);
return _responseHandler.Success<string>(null, "User suspended successfully");
}

return _responseHandler.BadRequest<string>("Failed to suspend user");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error suspending user {UserId}", userId);
return _responseHandler.ServerError<string>("Error suspending user");
}
}

public async Task<Response<string>> ActivateUserAsync(string userId)
{
try
{
var user = await _userManager.FindByIdAsync(userId);
if (user == null)
return _responseHandler.NotFound<string>("User not found");

user.IsDeleted = false;
var result = await _userManager.UpdateAsync(user);

if (result.Succeeded)
{
_logger.LogInformation("User {UserId} activated by admin", userId);
return _responseHandler.Success<string>(null, "User activated successfully");
}

return _responseHandler.BadRequest<string>("Failed to activate user");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error activating user {UserId}", userId);
return _responseHandler.ServerError<string>("Error activating user");
}
}

public async Task<Response<List<RecentBookingDto>>> GetRecentBookingsAsync(int limit = 10)
{
try
{
var requests = await _unitOfWork.ServiceRequests
.GetQueryable()
.Include(r => r.User)
.Include(r => r.ServiceProvider).ThenInclude(sp => sp.User)
.Include(r => r.Category)
.OrderByDescending(r => r.CreatedAt)
.Take(limit)
.ToListAsync();

var bookings = requests.Select(r => new RecentBookingDto
{
Id = r.Id,
UserName = $"{r.User.FirstName} {r.User.LastName}",
ProviderName = r.ServiceProvider != null
? $"{r.ServiceProvider.User.FirstName} {r.ServiceProvider.User.LastName}"
: "Not assigned",
Date = r.PreferredDate,
Shift = GetShiftName(r.ShiftType),
Amount = r.TotalPrice,
Status = GetBookingStatus(r.Status),
CategoryName = r.Category.Name
}).ToList();

return _responseHandler.Success(bookings, "Recent bookings retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving recent bookings");
return _responseHandler.ServerError<List<RecentBookingDto>>("Error retrieving bookings");
}
}

public async Task<Response<PaymentSummaryResponse>> GetPaymentTransactionsAsync()
{
try
{
var payments = await _unitOfWork.Payments
.GetQueryable()
.Include(p => p.ServiceRequest)
.ThenInclude(sr => sr.User)
.Include(p => p.ServiceRequest)
.ThenInclude(sr => sr.ServiceProvider)
.ThenInclude(sp => sp.User)
.OrderByDescending(p => p.CreatedAt)
.ToListAsync();

var transactions = payments.Select(p => new PaymentTransactionDto
{
Id = p.Id,
TransactionId = p.TransactionId ?? "N/A",
UserName = $"{p.ServiceRequest.User.FirstName} {p.ServiceRequest.User.LastName}",
ProviderName = p.ServiceRequest.ServiceProvider != null
? $"{p.ServiceRequest.ServiceProvider.User.FirstName} {p.ServiceRequest.ServiceProvider.User.LastName}"
: "Not assigned",
Date = p.CreatedAt,
Amount = p.Amount,
Status = GetPaymentStatus(p.PaymentStatus),
PaymentMethod = GetPaymentMethodName(p.PaymentMethod),
RequestId = p.ServiceRequestId
}).ToList();

var totalRevenue = payments
.Where(p => p.PaymentStatus == PaymentStatus.Completed)
.Sum(p => p.Amount);

var response = new PaymentSummaryResponse
{
TotalRevenue = totalRevenue,
Transactions = transactions
};

return _responseHandler.Success(response, "Payment transactions retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving payment transactions");
return _responseHandler.ServerError<PaymentSummaryResponse>("Error retrieving transactions");
}
}

private string GetShiftName(ShiftType shiftType)
{
return shiftType switch
{
ShiftType.ThreeHours => "3 Hours",
ShiftType.TwelveHours => "12 Hours",
ShiftType.TwentyFourHours => "24 Hours",
_ => "Unknown"
};
}

private string GetBookingStatus(ServiceRequestStatus status)
{
return status switch
{
ServiceRequestStatus.Completed => "completed",
ServiceRequestStatus.Pending => "pending",
ServiceRequestStatus.Cancelled => "cancelled",
ServiceRequestStatus.Accepted => "pending",
ServiceRequestStatus.Paid => "pending",
ServiceRequestStatus.InProgress => "pending",
ServiceRequestStatus.Rejected => "cancelled",
_ => "unknown"
};
}

private string GetPaymentStatus(PaymentStatus status)
{
return status switch
{
PaymentStatus.Completed => "completed",
PaymentStatus.Pending => "pending",
PaymentStatus.Failed => "failed",
_ => "unknown"
};
}

private string GetPaymentMethodName(PaymentMethod method)
{
return method switch
{
PaymentMethod.Cash => "Cash",
PaymentMethod.CreditCard => "Credit Card",
PaymentMethod.VodafoneCash => "Vodafone Cash",
_ => "Unknown"
};
}
}
}
```

# IAdminService.cs
```cs
﻿using ElAnis.Entities.DTO.Admin;

using ElAnis.Entities.Shared.Bases;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Admin
{
public interface IAdminService
{
Task<Response<ServiceProviderApplicationDetailDto>> GetServiceProviderApplicationByIdAsync(Guid id);
Task<Response<string>> ApproveServiceProviderApplicationAsync(Guid applicationId, ClaimsPrincipal adminClaims);
Task<Response<string>> RejectServiceProviderApplicationAsync(Guid applicationId, string rejectionReason, ClaimsPrincipal adminClaims);
Task<Response<string>> SuspendServiceProviderAsync(Guid serviceProviderId, string reason, ClaimsPrincipal adminClaims);
Task<Response<string>> ActivateServiceProviderAsync(Guid serviceProviderId, ClaimsPrincipal adminClaims);

Task<Response<PaginatedResult<ServiceProviderApplicationDto>>> GetServiceProviderApplicationsAsync(int page, int pageSize);
Task<Response<AdminDashboardStatsDto>> GetDashboardStatsAsync();
Task<Response<PaginatedResult<ServiceProviderDto>>> GetServiceProvidersAsync(int page, int pageSize);
Task<Response<PaginatedResult<UserManagementDto>>> GetUsersAsync(GetUsersRequest request);
Task<Response<string>> SuspendUserAsync(string userId);
Task<Response<string>> ActivateUserAsync(string userId);

Task<Response<List<RecentBookingDto>>> GetRecentBookingsAsync(int limit = 10);

Task<Response<PaymentSummaryResponse>> GetPaymentTransactionsAsync();
}
}
```

# CategoryService.cs
```cs
﻿// 2. Refactored Category Service using Repository Pattern

using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;
using Microsoft.Extensions.Logging;

namespace ElAnis.DataAccess.Services.Category
{
public class CategoryService : ICategoryService
{
private readonly IUnitOfWork _unitOfWork;
private readonly ResponseHandler _responseHandler;
private readonly ILogger<CategoryService> _logger;

public CategoryService(IUnitOfWork unitOfWork, ResponseHandler responseHandler, ILogger<CategoryService> logger)
{
_unitOfWork = unitOfWork;
_responseHandler = responseHandler;
_logger = logger;
}

public async Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync()
{
try
{
var categories = await _unitOfWork.Categories.GetAllAsync();
var sortedCategories = categories.OrderBy(c => c.Name).ToList();

var categoryDtos = sortedCategories.Select(c => new CategoryDtoResponse
{
Id = c.Id,
Name = c.Name,

Description = c.Description,
Icon = c.Icon,
IsActive = c.IsActive,
CreatedAt = c.CreatedAt
}).ToList();

return _responseHandler.Success(categoryDtos, "Categories retrieved successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving categories");
return _responseHandler.ServerError<List<CategoryDtoResponse>>("Error retrieving categories");
}
}

public async Task<Response<List<CategoryDtoResponse>>> GetActiveCategoriesAsync()
{
try
{
var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();

var categoryDtos = categories.Select(c => new CategoryDtoResponse
{
Id = c.Id,
Name = c.Name,

Description = c.Description,
Icon = c.Icon,
IsActive = c.IsActive,
CreatedAt = c.CreatedAt
}).ToList();

return _responseHandler.Success(categoryDtos, "Active categories retrieved successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving active categories");
return _responseHandler.ServerError<List<CategoryDtoResponse>>("Error retrieving categories");
}
}

public async Task<Response<CategoryDtoResponse>> GetCategoryByIdAsync(Guid id)
{
try
{
var category = await _unitOfWork.Categories.GetByIdAsync(id);

if (category == null)
return _responseHandler.NotFound<CategoryDtoResponse>("Category not found");

var categoryDto = new CategoryDtoResponse
{
Id = category.Id,
Name = category.Name,

Description = category.Description,
Icon = category.Icon,
IsActive = category.IsActive,
CreatedAt = category.CreatedAt
};

return _responseHandler.Success(categoryDto, "Category retrieved successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error retrieving category with ID: {CategoryId}", id);
return _responseHandler.ServerError<CategoryDtoResponse>("Error retrieving category");
}
}

public async Task<Response<CategoryDtoResponse>> CreateCategoryAsync(CreateCategoryRequest request)
{
try
{
var category = new ElAnis.Entities.Models.Category
{
Name = request.Name,

Description = request.Description,
Icon = request.Icon,
IsActive = request.IsActive
};

await _unitOfWork.Categories.AddAsync(category);
await _unitOfWork.CompleteAsync();

var categoryDto = new CategoryDtoResponse
{
Id = category.Id,
Name = category.Name,

Description = category.Description,
Icon = category.Icon,
IsActive = category.IsActive,
CreatedAt = category.CreatedAt
};

return _responseHandler.Created(categoryDto, "Category created successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error creating category");
return _responseHandler.ServerError<CategoryDtoResponse>("Error creating category");
}
}

public async Task<Response<CategoryDtoResponse>> UpdateCategoryAsync(Guid id, CreateCategoryRequest request)
{
try
{
var category = await _unitOfWork.Categories.GetByIdAsync(id);
if (category == null)
return _responseHandler.NotFound<CategoryDtoResponse>("Category not found");

category.Name = request.Name;

category.Description = request.Description;
category.Icon = request.Icon;
category.IsActive = request.IsActive;

_unitOfWork.Categories.Update(category);
await _unitOfWork.CompleteAsync();

var categoryDto = new CategoryDtoResponse
{
Id = category.Id,
Name = category.Name,
Description = category.Description,
Icon = category.Icon,
IsActive = category.IsActive,
CreatedAt = category.CreatedAt
};

return _responseHandler.Success(categoryDto, "Category updated successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error updating category with ID: {CategoryId}", id);
return _responseHandler.ServerError<CategoryDtoResponse>("Error updating category");
}
}

public async Task<Response<bool>> DeleteCategoryAsync(Guid id)
{
try
{
var category = await _unitOfWork.Categories.GetByIdAsync(id);
if (category == null)
return _responseHandler.NotFound<bool>("Category not found");

var hasProviders = await _unitOfWork.Categories.HasServiceProvidersAsync(id);
if (hasProviders)
{
category.IsActive = false;
_unitOfWork.Categories.Update(category);
}
else
{
_unitOfWork.Categories.Delete(category);
}

await _unitOfWork.CompleteAsync();
return _responseHandler.Success(true, "Category deleted successfully.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error deleting category with ID: {CategoryId}", id);
return _responseHandler.ServerError<bool>("Error deleting category");
}
}
}
}
```

# ICategoryService.cs
```cs
﻿using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;

namespace ElAnis.DataAccess.Services.Category
{
public interface ICategoryService
{
Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync();
Task<Response<List<CategoryDtoResponse>>> GetActiveCategoriesAsync();
Task<Response<CategoryDtoResponse>> GetCategoryByIdAsync(Guid id);
Task<Response<CategoryDtoResponse>> CreateCategoryAsync(CreateCategoryRequest request);
Task<Response<CategoryDtoResponse>> UpdateCategoryAsync(Guid id, CreateCategoryRequest request);
Task<Response<bool>> DeleteCategoryAsync(Guid id);
}
}
```

# CloudinaryImageUploadService.cs
```cs
﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using ElAnis.Utilities.Configurations;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ElAnis.DataAccess.Services.ImageUploading
{
public class CloudinaryImageUploadService : IImageUploadService
{
private readonly Cloudinary _cloudinary;
private readonly CloudinarySettings _cloudinarySettings;

public CloudinaryImageUploadService(IOptions<CloudinarySettings> cloudinaryOptions)
{
_cloudinarySettings = cloudinaryOptions.Value ?? throw new ArgumentNullException(nameof(cloudinaryOptions));
var account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);
_cloudinary = new Cloudinary(account);
_cloudinary.Api.Secure = true;
}

public async Task<string> UploadAsync(IFormFile file)
{
if (file == null || file.Length == 0)
throw new ArgumentException("File is empty or null");

await using var memoryStream = new MemoryStream();
await file.CopyToAsync(memoryStream);
memoryStream.Position = 0;

var uploadParams = new ImageUploadParams
{
File = new FileDescription(file.FileName, memoryStream)
};

var result = await _cloudinary.UploadAsync(uploadParams);

if (result == null)
throw new Exception("Upload result was null from Cloudinary.");

if (result.Error != null)
throw new Exception($"Cloudinary error occurred: {result.Error.Message}");

return result.Url?.ToString() ?? throw new Exception("Cloudinary returned empty URL.");
}
}
}
```

# IImageUploadService.cs
```cs
﻿using Microsoft.AspNetCore.Http;

namespace ElAnis.DataAccess.Services.ImageUploading
{
public interface IImageUploadService
{
Task<string> UploadAsync(IFormFile file);

}
}
```

# IOTPService.cs
```cs
﻿namespace ElAnis.DataAccess.Services.OTP
{
public interface IOTPService
{
Task<string> GenerateAndStoreOtpAsync(string userId);
Task<bool> ValidateOtpAsync(string userId, string otp);
}
}
```

# OTPServiceInMemory.cs
```cs
﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ElAnis.DataAccess.Services.OTP
{
public class OTPServiceInMemory : IOTPService
{
private readonly IMemoryCache _cache;
private readonly ILogger<OTPServiceInMemory> _logger;

public OTPServiceInMemory(IMemoryCache cache, ILogger<OTPServiceInMemory> logger)
{
_cache = cache;
_logger = logger;
}

public async Task<string> GenerateAndStoreOtpAsync(string userId)
{
var otp = GenerateOtp();

_cache.Set($"otp:{userId}", otp, TimeSpan.FromMinutes(5));

_logger.LogInformation("OTP generated and stored for UserId: {UserId}. Expiry: 5 Minutes", userId);

return await Task.FromResult(otp);
}

public async Task<bool> ValidateOtpAsync(string userId, string otp)
{
if (_cache.TryGetValue($"otp:{userId}", out string? storedOtp))
{
if (storedOtp == otp)
{
_cache.Remove($"otp:{userId}");
_logger.LogInformation("OTP validated successfully for UserId: {UserId}", userId);
return await Task.FromResult(true);
}

_logger.LogWarning("OTP validation failed: Invalid OTP for UserId: {UserId}", userId);
return await Task.FromResult(false);
}

_logger.LogWarning("OTP validation failed: No OTP found or expired for UserId: {UserId}", userId);
return await Task.FromResult(false);
}

private string GenerateOtp()
{
using var rng = RandomNumberGenerator.Create();
var bytes = new byte[4];
rng.GetBytes(bytes);
uint raw = BitConverter.ToUInt32(bytes, 0);
uint otp = raw % 1_000_000; // رقم من 000000 لغاية 999999

return otp.ToString("D6"); // يحافظ على 6 خانات
}
}
}
```

# OTPServiceRedis.cs
```cs
﻿using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

using StackExchange.Redis;

namespace ElAnis.DataAccess.Services.OTP
{
public class OTPServiceRedis : IOTPService
{
private readonly IDatabase _redis;
private readonly ILogger<OTPServiceRedis> _logger;

public OTPServiceRedis(IConnectionMultiplexer redis, ILogger<OTPServiceRedis> logger)
{
_redis = redis.GetDatabase();
_logger = logger;
}

public async Task<string> GenerateAndStoreOtpAsync(string userId)
{
var otp = GenerateOtp();

bool success = await _redis.StringSetAsync($"otp:{userId}", otp, TimeSpan.FromMinutes(5));
if (success)
_logger.LogInformation("OTP generated and stored for UserId: {UserId}. Expiry: 5 Minutes", userId);
else
_logger.LogWarning("Failed to store OTP in Redis for UserId: {UserId}", userId);

return otp;
}

public async Task<bool> ValidateOtpAsync(string userId, string otp)
{
var storedOtp = await _redis.StringGetAsync($"otp:{userId}");

if (storedOtp.IsNullOrEmpty)
{
_logger.LogWarning("OTP validation failed: No OTP found or expired for UserId: {UserId}", userId);
return false;
}

bool isValid = storedOtp == otp;

if (isValid)
{
await _redis.KeyDeleteAsync($"otp:{userId}");
_logger.LogInformation("OTP validated successfully for UserId: {UserId}", userId);
}
else
{
_logger.LogWarning("OTP validation failed: Invalid OTP for UserId: {UserId}", userId);
}

return isValid;
}

private string GenerateOtp()
{
using var rng = RandomNumberGenerator.Create();
var bytes = new byte[4];
rng.GetBytes(bytes);
uint raw = BitConverter.ToUInt32(bytes, 0);
uint otp = raw % 1_000_000; // number from 0 to 999999

return otp.ToString("D6");
}
}
}
```

# IPaymentService.cs
```cs
﻿using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Payment
{
public interface IPaymentService
{
Task<Response<PaymentResponse>> CreateStripeCheckoutSessionAsync(
CreatePaymentDto request,
ClaimsPrincipal userClaims);

Task<Response<PaymentResponse>> HandleStripeWebhookAsync(string json, string signature);

Task<Response<PaymentResponse>> GetPaymentByRequestIdAsync(Guid requestId);
}
}
```

# PaymentService.cs
```cs
﻿using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Configurations;
using ElAnis.Utilities.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;

namespace ElAnis.DataAccess.Services.Payment
{
public class PaymentService : IPaymentService
{
private readonly IUnitOfWork _unitOfWork;
private readonly ILogger<PaymentService> _logger;
private readonly ResponseHandler _responseHandler;
private readonly StripeSettings _stripeSettings;

public PaymentService(
IUnitOfWork unitOfWork,
ILogger<PaymentService> logger,
ResponseHandler responseHandler,
IOptions<StripeSettings> stripeSettings)
{
_unitOfWork = unitOfWork;
_logger = logger;
_responseHandler = responseHandler;
_stripeSettings = stripeSettings.Value;

StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
}

public async Task<Response<PaymentResponse>> CreateStripeCheckoutSessionAsync(
CreatePaymentDto request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<PaymentResponse>("User not authenticated");

var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(request.ServiceRequestId);
if (serviceRequest == null)
return _responseHandler.NotFound<PaymentResponse>("Service request not found");

if (serviceRequest.UserId != userId)
return _responseHandler.Forbidden<PaymentResponse>("You are not authorized to pay for this request");

if (serviceRequest.Status != ServiceRequestStatus.Accepted)
return _responseHandler.BadRequest<PaymentResponse>("Request must be accepted before payment");

var existingPayment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(request.ServiceRequestId);
if (existingPayment != null && existingPayment.PaymentStatus == PaymentStatus.Completed)
return _responseHandler.BadRequest<PaymentResponse>("Payment already completed");

var options = new SessionCreateOptions
{
PaymentMethodTypes = new List<string> { "card" },
LineItems = new List<SessionLineItemOptions>
{
new SessionLineItemOptions
{
PriceData = new SessionLineItemPriceDataOptions
{
Currency = "egp",
ProductData = new SessionLineItemPriceDataProductDataOptions
{
Name = $"Service: {serviceRequest.Category.Name}",
Description = $"Shift: {serviceRequest.ShiftType} on {serviceRequest.PreferredDate:yyyy-MM-dd}",
},
UnitAmount = (long)(serviceRequest.TotalPrice * 100),
},
Quantity = 1,
},
},
Mode = "payment",
SuccessUrl = $"{_stripeSettings.FrontendUrl}/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
CancelUrl = $"{_stripeSettings.FrontendUrl}/payment/cancel?request_id={request.ServiceRequestId}",
ClientReferenceId = request.ServiceRequestId.ToString(),
PaymentIntentData = new SessionPaymentIntentDataOptions
{
Metadata = new Dictionary<string, string>
{
{ "service_request_id", request.ServiceRequestId.ToString() },
{ "user_id", userId }
}
},
Metadata = new Dictionary<string, string>
{
{ "service_request_id", request.ServiceRequestId.ToString() },
{ "user_id", userId }
}
};

var service = new SessionService();
Session session = await service.CreateAsync(options);

if (existingPayment == null)
{
var payment = new ElAnis.Entities.Models.Payment
{
ServiceRequestId = request.ServiceRequestId,
Amount = serviceRequest.TotalPrice,
PaymentMethod = ElAnis.Utilities.Enum.PaymentMethod.CreditCard,
PaymentStatus = PaymentStatus.Pending,
TransactionId = session.Id,
CreatedAt = DateTime.UtcNow
};
await _unitOfWork.Payments.AddAsync(payment);
}
else
{
existingPayment.TransactionId = session.Id;
existingPayment.PaymentStatus = PaymentStatus.Pending;
_unitOfWork.Payments.Update(existingPayment);
}

serviceRequest.Status = ServiceRequestStatus.PaymentPending;
_unitOfWork.ServiceRequests.Update(serviceRequest);

await _unitOfWork.CompleteAsync();

_logger.LogInformation($"✅ Checkout session created: {session.Id} for request: {request.ServiceRequestId}");

var response = new PaymentResponse
{
Id = existingPayment?.Id ?? Guid.Empty,
ServiceRequestId = request.ServiceRequestId,
Amount = serviceRequest.TotalPrice,
PaymentMethod = ElAnis.Utilities.Enum.PaymentMethod.CreditCard,
PaymentStatus = PaymentStatus.Pending,
TransactionId = session.Id,
CheckoutUrl = session.Url,
CreatedAt = DateTime.UtcNow
};

return _responseHandler.Success(response, "Stripe checkout session created successfully");
}
catch (StripeException ex)
{
_logger.LogError(ex, "Stripe error while creating checkout session");
return _responseHandler.ServerError<PaymentResponse>($"Stripe error: {ex.Message}");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error creating Stripe checkout session");
return _responseHandler.ServerError<PaymentResponse>("Error creating payment session");
}
}

public async Task<Response<PaymentResponse>> HandleStripeWebhookAsync(string json, string signature)
{
try
{
var stripeEvent = EventUtility.ConstructEvent(
json,
signature,
_stripeSettings.WebhookSecret,
throwOnApiVersionMismatch: false
);

_logger.LogInformation($"✅ Webhook received: {stripeEvent.Type}");

switch (stripeEvent.Type)
{
case EventTypes.CheckoutSessionCompleted:
await HandleCheckoutSessionCompleted(stripeEvent);
break;

case EventTypes.CheckoutSessionExpired:
await HandleSessionExpired(stripeEvent);
break;

default:
_logger.LogInformation($"⚠️ Unhandled event: {stripeEvent.Type}");
break;
}

return _responseHandler.Success<PaymentResponse>(null, "Webhook processed");
}
catch (StripeException ex)
{
_logger.LogError(ex, "❌ Webhook verification failed");
return _responseHandler.BadRequest<PaymentResponse>($"Webhook error: {ex.Message}");
}
catch (Exception ex)
{
_logger.LogError(ex, "❌ Webhook processing error");
return _responseHandler.ServerError<PaymentResponse>("Error processing webhook");
}
}

private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
{
try
{
var session = stripeEvent.Data.Object as Session;
if (session == null)
{
_logger.LogWarning("⚠️ Invalid session data");
return;
}

_logger.LogInformation($"💳 Processing payment for session: {session.Id}");
_logger.LogInformation($"📋 ClientReferenceId: {session.ClientReferenceId}");
_logger.LogInformation($"💰 Payment status: {session.PaymentStatus}");

if (string.IsNullOrEmpty(session.ClientReferenceId))
{
_logger.LogError($"❌ No ClientReferenceId in session: {session.Id}");
return;
}

if (!Guid.TryParse(session.ClientReferenceId, out var serviceRequestId))
{
_logger.LogError($"❌ Invalid ClientReferenceId: {session.ClientReferenceId}");
return;
}

var payment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(serviceRequestId);
if (payment == null)
{
_logger.LogError($"❌ Payment not found for request: {serviceRequestId}");
return;
}

_logger.LogInformation($"📊 Current payment status: {payment.PaymentStatus}");

payment.PaymentStatus = PaymentStatus.Completed;
payment.PaidAt = DateTime.UtcNow;
payment.TransactionId = session.PaymentIntentId ?? session.Id;
payment.PaymentGatewayResponse = JsonSerializer.Serialize(session);

_unitOfWork.Payments.Update(payment);

var serviceRequest = await _unitOfWork.ServiceRequests.GetByIdAsync(serviceRequestId);
if (serviceRequest != null)
{
_logger.LogInformation($"📊 Current request status: {serviceRequest.Status}");

serviceRequest.Status = ServiceRequestStatus.Paid;
_unitOfWork.ServiceRequests.Update(serviceRequest);

_logger.LogInformation($"✅ Request status updated to: Paid");
}
else
{
_logger.LogError($"❌ ServiceRequest not found: {serviceRequestId}");
}

await _unitOfWork.CompleteAsync();

_logger.LogInformation($"🎉 Payment completed successfully for request: {serviceRequestId}");
}
catch (Exception ex)
{
_logger.LogError(ex, "❌ Error in HandleCheckoutSessionCompleted");
}
}

private async Task HandleSessionExpired(Event stripeEvent)
{
try
{
var session = stripeEvent.Data.Object as Session;
if (session == null) return;

_logger.LogWarning($"⏱️ Session expired: {session.Id}");

if (string.IsNullOrEmpty(session.ClientReferenceId) ||
!Guid.TryParse(session.ClientReferenceId, out var serviceRequestId))
{
return;
}

var payment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(serviceRequestId);
if (payment != null && payment.PaymentStatus == PaymentStatus.Pending)
{
payment.PaymentStatus = PaymentStatus.Failed;
_unitOfWork.Payments.Update(payment);

var serviceRequest = await _unitOfWork.ServiceRequests.GetByIdAsync(serviceRequestId);
if (serviceRequest != null)
{
serviceRequest.Status = ServiceRequestStatus.Accepted;
_unitOfWork.ServiceRequests.Update(serviceRequest);
}

await _unitOfWork.CompleteAsync();
_logger.LogInformation($"✅ Session expiry handled for request: {serviceRequestId}");
}
}
catch (Exception ex)
{
_logger.LogError(ex, "❌ Error in HandleSessionExpired");
}
}

public async Task<Response<PaymentResponse>> GetPaymentByRequestIdAsync(Guid requestId)
{
try
{
var payment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(requestId);
if (payment == null)
return _responseHandler.NotFound<PaymentResponse>("Payment not found");

var response = MapToResponse(payment);
return _responseHandler.Success(response, "Payment retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting payment");
return _responseHandler.ServerError<PaymentResponse>("Error retrieving payment");
}
}

private PaymentResponse MapToResponse(ElAnis.Entities.Models.Payment payment)
{
return new PaymentResponse
{
Id = payment.Id,
ServiceRequestId = payment.ServiceRequestId,
Amount = payment.Amount,
PaymentMethod = payment.PaymentMethod,
PaymentStatus = payment.PaymentStatus,
TransactionId = payment.TransactionId,
CreatedAt = payment.CreatedAt,
PaidAt = payment.PaidAt
};
}
}
}
```

# IServicePricingService.cs
```cs
﻿using ElAnis.Entities.DTO.ServicePricing;
using ElAnis.Entities.Shared.Bases;

using System.Security.Claims;

namespace ElAnis.DataAccess.Services.ServicePricing
{
public interface IServicePricingService
{
Task<Response<ServicePricingResponse>> CreateAsync(CreateServicePricingRequest request, ClaimsPrincipal userClaims);
Task<Response<List<ServicePricingResponse>>> CreateBulkAsync(BulkServicePricingRequest request, ClaimsPrincipal userClaims);
Task<Response<ServicePricingResponse>> UpdateAsync(Guid id, UpdateServicePricingRequest request, ClaimsPrincipal userClaims);
Task<Response<string>> DeleteAsync(Guid id, ClaimsPrincipal userClaims);

Task<Response<ServicePricingResponse>> GetByIdAsync(Guid id);
Task<Response<List<ServicePricingResponse>>> GetByCategoryIdAsync(Guid categoryId);
Task<Response<List<CategoryWithPricingResponse>>> GetAllCategoriesWithPricingAsync();
Task<Response<List<ServicePricingResponse>>> GetActivePricingAsync();
}

}
```

# ServicePricingService.cs
```cs
﻿using ElAnis.Entities.DTO.ServicePricing;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.ServicePricing
{
public class ServicePricingService : IServicePricingService
{
private readonly IUnitOfWork _unitOfWork;
private readonly ILogger<ServicePricingService> _logger;
private readonly ResponseHandler _responseHandler;

public ServicePricingService(
IUnitOfWork unitOfWork,
ILogger<ServicePricingService> logger,
ResponseHandler responseHandler)
{
_unitOfWork = unitOfWork;
_logger = logger;
_responseHandler = responseHandler;
}

public async Task<Response<ServicePricingResponse>> CreateAsync(
CreateServicePricingRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
if (category == null)
return _responseHandler.NotFound<ServicePricingResponse>("Category not found");

var exists = await _unitOfWork.ServicePricings
.ExistsForShiftTypeAsync(request.CategoryId, request.ShiftType);

if (exists)
return _responseHandler.BadRequest<ServicePricingResponse>(
$"Pricing for {GetShiftTypeName(request.ShiftType)} already exists for this category");

var pricing = new ElAnis.Entities.Models.ServicePricing
{
CategoryId = request.CategoryId,
ShiftType = request.ShiftType,
PricePerShift = request.PricePerShift,
Description = request.Description,
IsActive = request.IsActive,
UpdatedBy = userId
};

await _unitOfWork.ServicePricings.AddAsync(pricing);
await _unitOfWork.CompleteAsync();

var response = MapToResponse(pricing, category);
return _responseHandler.Created(response, "Service pricing created successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error creating service pricing");
return _responseHandler.ServerError<ServicePricingResponse>("Error creating service pricing");
}
}

public async Task<Response<List<ServicePricingResponse>>> CreateBulkAsync(
BulkServicePricingRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
if (category == null)
return _responseHandler.NotFound<List<ServicePricingResponse>>("Category not found");

var duplicateShiftTypes = request.Pricings
.GroupBy(p => p.ShiftType)
.Where(g => g.Count() > 1)
.Select(g => GetShiftTypeName(g.Key))
.ToList();

if (duplicateShiftTypes.Any())
return _responseHandler.BadRequest<List<ServicePricingResponse>>(
$"Duplicate shift types found: {string.Join(", ", duplicateShiftTypes)}");

var existingPricings = await _unitOfWork.ServicePricings
.GetByCategoryIdAsync(request.CategoryId);

var existingShiftTypes = existingPricings.Select(p => p.ShiftType).ToHashSet();
var conflictingTypes = request.Pricings
.Where(p => existingShiftTypes.Contains(p.ShiftType))
.Select(p => GetShiftTypeName(p.ShiftType))
.ToList();

if (conflictingTypes.Any())
return _responseHandler.BadRequest<List<ServicePricingResponse>>(
$"Pricing already exists for: {string.Join(", ", conflictingTypes)}");

var newPricings = request.Pricings.Select(p => new ElAnis.Entities.Models.ServicePricing
{
CategoryId = request.CategoryId,
ShiftType = p.ShiftType,
PricePerShift = p.PricePerShift,
Description = p.Description,
IsActive = true,
UpdatedBy = userId
}).ToList();

await _unitOfWork.ServicePricings.AddRangeAsync(newPricings);
await _unitOfWork.CompleteAsync();

var responses = newPricings.Select(p => MapToResponse(p, category)).ToList();
return _responseHandler.Created(responses, $"{responses.Count} pricing records created successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error creating bulk service pricing");
return _responseHandler.ServerError<List<ServicePricingResponse>>("Error creating bulk service pricing");
}
}

public async Task<Response<ServicePricingResponse>> UpdateAsync(
Guid id,
UpdateServicePricingRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

var pricing = await _unitOfWork.ServicePricings.GetByIdAsync(id);
if (pricing == null)
return _responseHandler.NotFound<ServicePricingResponse>("Service pricing not found");

var category = await _unitOfWork.Categories.GetByIdAsync(pricing.CategoryId);
if (category == null)
return _responseHandler.NotFound<ServicePricingResponse>("Category not found");

pricing.PricePerShift = request.PricePerShift;
pricing.Description = request.Description;
pricing.IsActive = request.IsActive;
pricing.UpdatedAt = DateTime.UtcNow;
pricing.UpdatedBy = userId;

_unitOfWork.ServicePricings.Update(pricing);
await _unitOfWork.CompleteAsync();

var response = MapToResponse(pricing, category);
return _responseHandler.Success(response, "Service pricing updated successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error updating service pricing");
return _responseHandler.ServerError<ServicePricingResponse>("Error updating service pricing");
}
}

public async Task<Response<string>> DeleteAsync(Guid id, ClaimsPrincipal userClaims)
{
try
{
var pricing = await _unitOfWork.ServicePricings.GetByIdAsync(id);
if (pricing == null)
return _responseHandler.NotFound<string>("Service pricing not found");

pricing.IsActive = false;
pricing.UpdatedAt = DateTime.UtcNow;
pricing.UpdatedBy = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

_unitOfWork.ServicePricings.Update(pricing);
await _unitOfWork.CompleteAsync();

return _responseHandler.Success<string>(null, "Service pricing deleted successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error deleting service pricing");
return _responseHandler.ServerError<string>("Error deleting service pricing");
}
}

public async Task<Response<ServicePricingResponse>> GetByIdAsync(Guid id)
{
try
{
var pricing = await _unitOfWork.ServicePricings
.GetQueryable()
.Where(p => p.Id == id)
.Select(p => new ServicePricingResponse
{
Id = p.Id,
CategoryId = p.CategoryId,
CategoryName = p.Category.Name,
ShiftType = p.ShiftType,
ShiftTypeName = GetShiftTypeName(p.ShiftType),
PricePerShift = p.PricePerShift,
Description = p.Description,
IsActive = p.IsActive,
CreatedAt = p.CreatedAt,
UpdatedAt = p.UpdatedAt,
UpdatedBy = p.UpdatedBy
})
.FirstOrDefaultAsync();

if (pricing == null)
return _responseHandler.NotFound<ServicePricingResponse>("Service pricing not found");

return _responseHandler.Success(pricing, "Service pricing retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting service pricing");
return _responseHandler.ServerError<ServicePricingResponse>("Error retrieving service pricing");
}
}

public async Task<Response<List<ServicePricingResponse>>> GetByCategoryIdAsync(Guid categoryId)
{
try
{
var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
if (category == null)
return _responseHandler.NotFound<List<ServicePricingResponse>>("Category not found");

var pricings = await _unitOfWork.ServicePricings.GetByCategoryIdAsync(categoryId);
var responses = pricings.Select(p => MapToResponse(p, category)).ToList();

return _responseHandler.Success(responses, "Pricing retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting pricing by category");
return _responseHandler.ServerError<List<ServicePricingResponse>>("Error retrieving pricing");
}
}

public async Task<Response<List<CategoryWithPricingResponse>>> GetAllCategoriesWithPricingAsync()
{
try
{
var categories = await _unitOfWork.Categories.GetAllAsync();
var pricingsDict = await _unitOfWork.ServicePricings.GetAllCategoriesWithPricingAsync();

var responses = categories.Select(c => new CategoryWithPricingResponse
{
CategoryId = c.Id,
CategoryName = c.Name,
CategoryDescription = c.Description,
CategoryIcon = c.Icon,
CategoryIsActive = c.IsActive,
Pricing = pricingsDict.ContainsKey(c.Id)
? pricingsDict[c.Id].Select(p => MapToResponse(p, c)).ToList()
: new List<ServicePricingResponse>()
}).ToList();

return _responseHandler.Success(responses, "Categories with pricing retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting all categories with pricing");
return _responseHandler.ServerError<List<CategoryWithPricingResponse>>("Error retrieving data");
}
}

public async Task<Response<List<ServicePricingResponse>>> GetActivePricingAsync()
{
try
{
var pricings = await _unitOfWork.ServicePricings.GetActivePricingAsync();
var responses = pricings.Select(p => MapToResponse(p, p.Category)).ToList();

return _responseHandler.Success(responses, "Active pricing retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting active pricing");
return _responseHandler.ServerError<List<ServicePricingResponse>>("Error retrieving active pricing");
}
}

private ServicePricingResponse MapToResponse(ElAnis.Entities.Models.ServicePricing pricing, ElAnis.Entities.Models.Category category)
{
return new ServicePricingResponse
{
Id = pricing.Id,
CategoryId = pricing.CategoryId,
CategoryName = category.Name,
ShiftType = pricing.ShiftType,
ShiftTypeName = GetShiftTypeName(pricing.ShiftType),
PricePerShift = pricing.PricePerShift,
Description = pricing.Description,
IsActive = pricing.IsActive,
CreatedAt = pricing.CreatedAt,
UpdatedAt = pricing.UpdatedAt,
UpdatedBy = pricing.UpdatedBy
};
}

private string GetShiftTypeName(ShiftType shiftType)
{
return shiftType switch
{
ShiftType.ThreeHours => "3 Hours",
ShiftType.TwelveHours => "12 Hours",
ShiftType.TwentyFourHours => "24 Hours",
_ => "Unknown"
};
}
}
}
```

# IServiceProviderService.cs
```cs
﻿using System.Security.Claims;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.DTO.Availability;
using ElAnis.Entities.DTO.Provider;
using ElAnis.Entities.DTO.ServiceProviderProfile;
using ElAnis.Entities.DTO.WorkingArea;
using ElAnis.Entities.Shared.Bases;

namespace ElAnis.DataAccess.Services.ServiceProvider
{
public interface IServiceProviderService
{

Task<Response<ApplicationStatusResponse>> GetApplicationStatusAsync(ClaimsPrincipal userClaims);
Task<Response<ProviderDashboardResponse>> GetDashboardAsync(ClaimsPrincipal userClaims);

Task<Response<ProviderProfileResponse>> GetProfileAsync(ClaimsPrincipal userClaims);
Task<Response<ProviderProfileResponse>> UpdateProfileAsync(UpdateProviderProfileRequest request, ClaimsPrincipal userClaims);
Task<Response<string>> ToggleAvailabilityAsync(ToggleAvailabilityRequest request, ClaimsPrincipal userClaims);

Task<Response<List<Entities.DTO.WorkingArea.WorkingAreaDto>>> GetWorkingAreasAsync(ClaimsPrincipal userClaims);
Task<Response<Entities.DTO.WorkingArea.WorkingAreaDto>> AddWorkingAreaAsync(AddWorkingAreaRequest request, ClaimsPrincipal userClaims);
Task<Response<string>> DeleteWorkingAreaAsync(Guid workingAreaId, ClaimsPrincipal userClaims);
Task<Response<AvailabilityCalendarResponse>> GetAvailabilityCalendarAsync(DateTime startDate, DateTime endDate, ClaimsPrincipal userClaims);
Task<Response<Entities.DTO.Availability.AvailabilityDto>> AddAvailabilityAsync(AddAvailabilityRequest request, ClaimsPrincipal userClaims);
Task<Response<Entities.DTO.Availability.AvailabilityDto>> UpdateAvailabilityAsync(UpdateAvailabilityRequest request, ClaimsPrincipal userClaims);
Task<Response<string>> DeleteAvailabilityAsync(Guid availabilityId, ClaimsPrincipal userClaims);
Task<Response<string>> AddBulkAvailabilityAsync(BulkAvailabilityRequest request, ClaimsPrincipal userClaims);
Task<Response<PaginatedResult<ProviderSummaryResponse>>> SearchProvidersAsync(GetProvidersRequest request);
Task<Response<ProviderDetailResponse>> GetProviderDetailAsync(Guid providerId);
}
}
```

# ServiceProviderService.cs
```cs
﻿using ElAnis.DataAccess;
using ElAnis.DataAccess.Services.ServiceProvider;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.DTO.Availability;
using ElAnis.Entities.DTO.Provider;
using ElAnis.Entities.DTO.ServiceProviderProfile;
using ElAnis.Entities.DTO.WorkingArea;
using ElAnis.Entities.Models;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class ServiceProviderService : IServiceProviderService
{
private readonly IUnitOfWork _unitOfWork;
private readonly ILogger<ServiceProvider> _logger;
private readonly ResponseHandler _responseHandler;

public ServiceProviderService(
IUnitOfWork unitOfWork,
ILogger<ServiceProvider> logger,
ResponseHandler responseHandler)
{
_unitOfWork = unitOfWork;
_logger = logger;
_responseHandler = responseHandler;
}

public async Task<Response<ApplicationStatusResponse>> GetApplicationStatusAsync(ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ApplicationStatusResponse>("User not authenticated");

var application = await _unitOfWork.ServiceProviderApplications
.FindSingleAsync(a => a.UserId == userId);

if (application == null)
return _responseHandler.NotFound<ApplicationStatusResponse>("No application found");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

var statusClaim = userClaims.FindFirst("ServiceProviderStatus")?.Value;
bool needsTokenRefresh = false;
string? nextAction = null;
string? refreshEndpoint = null;

if (application.Status == ServiceProviderApplicationStatus.Approved)
{
if (profile == null)
{
nextAction = "Your application was approved but profile creation failed. Please contact support.";
}
else if (statusClaim != "Approved")
{
needsTokenRefresh = true;
nextAction = "Please refresh your access token to access the provider dashboard.";
refreshEndpoint = "/api/auth/refresh-token";
}
else
{
nextAction = "You can now access your provider dashboard.";
}
}

var response = new ApplicationStatusResponse
{
ApplicationId = application.Id,
Status = application.Status,
StatusText = GetStatusText(application.Status),
Message = GetStatusMessage(application.Status),
RejectionReason = application.RejectionReason,
CreatedAt = application.CreatedAt,
ReviewedAt = application.ReviewedAt,
CanReapply = application.Status == ServiceProviderApplicationStatus.Rejected,
HasProfile = profile != null,
NeedsTokenRefresh = needsTokenRefresh,
NextAction = nextAction,
RefreshTokenEndpoint = refreshEndpoint
};

return _responseHandler.Success(response, "Application status retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting application status");
return _responseHandler.ServerError<ApplicationStatusResponse>("Error retrieving application status");
}
}

public async Task<Response<ProviderDashboardResponse>> GetDashboardAsync(ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ProviderDashboardResponse>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.GetQueryable()
.Include(p => p.User)
.Include(p => p.Categories).ThenInclude(c => c.Category)
.Include(p => p.WorkingAreas)
.Include(p => p.ServiceRequests)
.FirstOrDefaultAsync(p => p.UserId == userId);

if (profile == null)
{
return _responseHandler.Forbidden<ProviderDashboardResponse>(
"Your application is still pending or has been rejected. " +
"Please check your application status at /api/Provider/application-status");
}

if (profile.Status != ServiceProviderStatus.Approved)
{
return _responseHandler.Forbidden<ProviderDashboardResponse>(
$"Your provider status is {profile.Status}. Only approved providers can access the dashboard. " +
"If you were recently approved, please log out and log in again.");
}

var now = DateTime.UtcNow;
var startOfMonth = new DateTime(now.Year, now.Month, 1);

var pendingRequests = await _unitOfWork.Repository<ServiceRequest>()
.CountAsync(r => r.ServiceProviderId == profile.Id && r.Status == ServiceRequestStatus.Pending);

var upcomingJobs = await _unitOfWork.Repository<ServiceRequest>()
.CountAsync(r => r.ServiceProviderId == profile.Id
&& (r.Status == ServiceRequestStatus.Accepted || r.Status == ServiceRequestStatus.Paid)
&& r.PreferredDate >= now);

var currentMonthEarnings = await _unitOfWork.Repository<ServiceRequest>()
.GetQueryable()
.Where(r => r.ServiceProviderId == profile.Id
&& r.Status == ServiceRequestStatus.Completed
&& r.CompletedAt >= startOfMonth)
.SumAsync(r => r.TotalPrice);

var recentRequests = await _unitOfWork.Repository<ServiceRequest>()
.GetQueryable()
.Where(r => r.ServiceProviderId == profile.Id)
.OrderByDescending(r => r.CreatedAt)
.Take(5)
.Include(r => r.User)
.Include(r => r.Category)
.Select(r => new ServiceRequestSummary
{
Id = r.Id,
ClientName = r.User.FirstName + " " + r.User.LastName,
CategoryName = r.Category.Name,
PreferredDate = r.PreferredDate,
ShiftType = r.ShiftType,
ShiftTypeName = r.ShiftType.ToString(),
Status = r.Status,
StatusText = r.Status.ToString(),
Price = r.TotalPrice,
Address = r.Address,
Governorate = r.Governorate
})
.ToListAsync();

var upcomingJobsList = await _unitOfWork.Repository<ServiceRequest>()
.GetQueryable()
.Where(r => r.ServiceProviderId == profile.Id
&& (r.Status == ServiceRequestStatus.Accepted || r.Status == ServiceRequestStatus.Paid)
&& r.PreferredDate >= now)
.OrderBy(r => r.PreferredDate)
.Take(5)
.Include(r => r.User)
.Include(r => r.Category)
.Select(r => new ServiceRequestSummary
{
Id = r.Id,
ClientName = r.User.FirstName + " " + r.User.LastName,
CategoryName = r.Category.Name,
PreferredDate = r.PreferredDate,
ShiftType = r.ShiftType,
ShiftTypeName = r.ShiftType.ToString(),
Status = r.Status,
StatusText = r.Status.ToString(),
Price = r.TotalPrice,
Address = r.Address,
Governorate = r.Governorate
})
.ToListAsync();

var response = new ProviderDashboardResponse
{
ProfileId = profile.Id,
FullName = profile.User.FirstName + " " + profile.User.LastName,
Email = profile.User.Email ?? string.Empty,
ProfilePicture = profile.User.ProfilePicture,
IsAvailable = profile.IsAvailable,
Status = profile.Status,
Statistics = new DashboardStatistics
{
CompletedJobs = profile.CompletedJobs,
PendingRequests = pendingRequests,
UpcomingJobs = upcomingJobs,
TotalEarnings = profile.TotalEarnings,
CurrentMonthEarnings = currentMonthEarnings,
AverageRating = profile.AverageRating,
TotalReviews = profile.TotalReviews,
WorkedDays = profile.WorkedDays
},
RecentRequests = recentRequests,
UpcomingJobs = upcomingJobsList,
Categories = profile.Categories.Select(c => new CategorySummary
{
Id = c.CategoryId,
Name = c.Category.Name,
Icon = c.Category.Icon
}).ToList(),
WorkingAreas = profile.WorkingAreas
.Where(w => w.IsActive)
.Select(w => w.Governorate)
.Distinct()
.ToList()
};

return _responseHandler.Success(response, "Dashboard retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting provider dashboard");
return _responseHandler.ServerError<ProviderDashboardResponse>("Error retrieving dashboard");
}
}
public async Task<Response<ProviderProfileResponse>> GetProfileAsync(ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ProviderProfileResponse>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.GetQueryable()
.Include(p => p.User)
.Include(p => p.Categories).ThenInclude(c => c.Category)
.Include(p => p.WorkingAreas)
.FirstOrDefaultAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<ProviderProfileResponse>("Profile not found");

var response = new ProviderProfileResponse
{
Id = profile.Id,
UserId = profile.UserId,
FirstName = profile.User.FirstName,
LastName = profile.User.LastName,
Email = profile.User.Email ?? string.Empty,
PhoneNumber = profile.User.PhoneNumber ?? string.Empty,
ProfilePicture = profile.User.ProfilePicture,
Bio = profile.Bio,
Experience = profile.Experience,
NationalId = profile.NationalId,
IsAvailable = profile.IsAvailable,
Status = profile.Status,
CompletedJobs = profile.CompletedJobs,
TotalEarnings = profile.TotalEarnings,
AverageRating = profile.AverageRating,
TotalReviews = profile.TotalReviews,
Categories = profile.Categories.Select(c => new CategorySummary
{
Id = c.CategoryId,
Name = c.Category.Name,
Icon = c.Category.Icon
}).ToList(),
WorkingAreas = profile.WorkingAreas
.Where(w => w.IsActive)
.Select(w => new ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto
{
Id = w.Id,
Governorate = w.Governorate,
City = w.City,
District = w.District,
IsActive = w.IsActive
}).ToList()
};

return _responseHandler.Success(response , "Profile retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting provider profile");
return _responseHandler.ServerError<ProviderProfileResponse>("Error retrieving profile");
}
}

public async Task<Response<ProviderProfileResponse>> UpdateProfileAsync(
UpdateProviderProfileRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ProviderProfileResponse>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<ProviderProfileResponse>("Profile not found");

if (!string.IsNullOrEmpty(request.Bio))
profile.Bio = request.Bio;

if (!string.IsNullOrEmpty(request.Experience))
profile.Experience = request.Experience;

if (request.ProfilePicture != null)
{
}

_unitOfWork.ServiceProviderProfiles.Update(profile);
await _unitOfWork.CompleteAsync();

return await GetProfileAsync(userClaims);
}
catch (Exception ex)
{
_logger.LogError(ex, "Error updating provider profile");
return _responseHandler.ServerError<ProviderProfileResponse>("Error updating profile");
}
}

public async Task<Response<string>> ToggleAvailabilityAsync(
ToggleAvailabilityRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<string>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<string>("Profile not found");

profile.IsAvailable = request.IsAvailable;
_unitOfWork.ServiceProviderProfiles.Update(profile);
await _unitOfWork.CompleteAsync();

var message = request.IsAvailable ? "You are now available for work" : "You are now unavailable";
return _responseHandler.Success<string>(null, message);
}
catch (Exception ex)
{
_logger.LogError(ex, "Error toggling availability");
return _responseHandler.ServerError<string>("Error updating availability");
}
}

public async Task<Response<List<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>>> GetWorkingAreasAsync(ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<List<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<List<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>>("Profile not found");

var workingAreas = await _unitOfWork.ProviderWorkingAreas
.GetProviderWorkingAreasAsync(profile.Id);

var response = workingAreas.Select(w => new ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto
{
Id = w.Id,
Governorate = w.Governorate,
City = w.City,
District = w.District,
IsActive = w.IsActive
}).ToList();

return _responseHandler.Success(response, "Working Area retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting working areas");
return _responseHandler.ServerError<List<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>>("Error retrieving working areas");
}
}

public async Task<Response<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>> AddWorkingAreaAsync(
AddWorkingAreaRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>("Profile not found");

var exists = await _unitOfWork.ProviderWorkingAreas
.IsGovernorateExistsAsync(profile.Id, request.Governorate);

if (exists)
return _responseHandler.BadRequest<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>("This governorate already exists in your working areas");

var workingArea = new ProviderWorkingArea
{
ServiceProviderId = profile.Id,
Governorate = request.Governorate,
City = request.City,
District = request.District,
IsActive = true
};

await _unitOfWork.ProviderWorkingAreas.AddAsync(workingArea);
await _unitOfWork.CompleteAsync();

var response = new ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto
{
Id = workingArea.Id,
Governorate = workingArea.Governorate,
City = workingArea.City,
District = workingArea.District,
IsActive = workingArea.IsActive
};

return _responseHandler.Created(response, "Working area added successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error adding working area");
return _responseHandler.ServerError<ElAnis.Entities.DTO.WorkingArea.WorkingAreaDto>("Error adding working area");
}
}

public async Task<Response<string>> DeleteWorkingAreaAsync(Guid workingAreaId, ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<string>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<string>("Profile not found");

var workingArea = await _unitOfWork.ProviderWorkingAreas
.FindSingleAsync(w => w.Id == workingAreaId && w.ServiceProviderId == profile.Id);

if (workingArea == null)
return _responseHandler.NotFound<string>("Working area not found");

workingArea.IsActive = false;
_unitOfWork.ProviderWorkingAreas.Update(workingArea);
await _unitOfWork.CompleteAsync();

return _responseHandler.Success<string>(null, "Working area deleted successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error deleting working area");
return _responseHandler.ServerError<string>("Error deleting working area");
}
}

public async Task<Response<AvailabilityCalendarResponse>> GetAvailabilityCalendarAsync(
DateTime startDate,
DateTime endDate,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<AvailabilityCalendarResponse>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<AvailabilityCalendarResponse>("Profile not found");

var availability = await _unitOfWork.ProviderAvailabilities
.GetProviderAvailabilityAsync(profile.Id, startDate, endDate);

var bookedRequests = await _unitOfWork.Repository<ServiceRequest>()
.GetQueryable()
.Where(r => r.ServiceProviderId == profile.Id
&& r.PreferredDate.Date >= startDate.Date
&& r.PreferredDate.Date <= endDate.Date
&& (r.Status == ServiceRequestStatus.Accepted
|| r.Status == ServiceRequestStatus.Paid
|| r.Status == ServiceRequestStatus.InProgress))
.Include(r => r.User)
.Include(r => r.Category)
.Select(r => new ServiceRequestSummary
{
Id = r.Id,
ClientName = r.User.FirstName + " " + r.User.LastName,
CategoryName = r.Category.Name,
PreferredDate = r.PreferredDate,
ShiftType = r.ShiftType,
ShiftTypeName = r.ShiftType.ToString(),
Status = r.Status,
StatusText = r.Status.ToString(),
Price = r.TotalPrice,
Address = r.Address,
Governorate = r.Governorate
})
.ToListAsync();

var response = new AvailabilityCalendarResponse
{
Availability = availability.Select(a => new ElAnis.Entities.DTO.Availability.AvailabilityDto
{
Id = a.Id,
Date = a.Date,
IsAvailable = a.IsAvailable,
AvailableShift = a.AvailableShift,
Notes = a.Notes
}).ToList(),
BookedDates = bookedRequests
};

return _responseHandler.Success(response, "Availability  retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting availability calendar");
return _responseHandler.ServerError<AvailabilityCalendarResponse>("Error retrieving calendar");
}
}

public async Task<Response<ElAnis.Entities.DTO.Availability.AvailabilityDto>> AddAvailabilityAsync(
AddAvailabilityRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ElAnis.Entities.DTO.Availability.AvailabilityDto>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<ElAnis.Entities.DTO.Availability.AvailabilityDto>("Profile not found");

var existing = await _unitOfWork.ProviderAvailabilities
.GetByDateAndShiftAsync(profile.Id, request.Date, request.AvailableShift);

if (existing != null)
return _responseHandler.BadRequest<ElAnis.Entities.DTO.Availability.AvailabilityDto>(
$"Availability for {request.Date:yyyy-MM-dd} and shift {request.AvailableShift} already exists. Please update it instead.");

var availability = new ProviderAvailability
{
ServiceProviderId = profile.Id,
Date = request.Date.Date,
IsAvailable = request.IsAvailable,
AvailableShift = request.AvailableShift,
Notes = request.Notes
};

await _unitOfWork.ProviderAvailabilities.AddAsync(availability);
await _unitOfWork.CompleteAsync();

var response = new ElAnis.Entities.DTO.Availability.AvailabilityDto
{
Id = availability.Id,
Date = availability.Date,
IsAvailable = availability.IsAvailable,
AvailableShift = availability.AvailableShift,
Notes = availability.Notes
};

return _responseHandler.Created(response, "Availability added successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error adding availability");
return _responseHandler.ServerError<ElAnis.Entities.DTO.Availability.AvailabilityDto>("Error adding availability");
}
}
public async Task<Response<ElAnis.Entities.DTO.Availability.AvailabilityDto>> UpdateAvailabilityAsync(
UpdateAvailabilityRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ElAnis.Entities.DTO.Availability.AvailabilityDto>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<ElAnis.Entities.DTO.Availability.AvailabilityDto>("Profile not found");

var availability = await _unitOfWork.ProviderAvailabilities
.FindSingleAsync(a => a.Id == request.Id && a.ServiceProviderId == profile.Id);

if (availability == null)
return _responseHandler.NotFound<ElAnis.Entities.DTO.Availability.AvailabilityDto>("Availability not found");

availability.IsAvailable = request.IsAvailable;
availability.AvailableShift = request.AvailableShift;
availability.Notes = request.Notes;
availability.UpdatedAt = DateTime.UtcNow;

_unitOfWork.ProviderAvailabilities.Update(availability);
await _unitOfWork.CompleteAsync();

var response = new ElAnis.Entities.DTO.Availability.AvailabilityDto
{
Id = availability.Id,
Date = availability.Date,
IsAvailable = availability.IsAvailable,
AvailableShift = availability.AvailableShift,
Notes = availability.Notes
};

return _responseHandler.Success(response, "Availability updated successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error updating availability");
return _responseHandler.ServerError<ElAnis.Entities.DTO.Availability.AvailabilityDto>("Error updating availability");
}
}

public async Task<Response<string>> DeleteAvailabilityAsync(Guid availabilityId, ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<string>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<string>("Profile not found");

var availability = await _unitOfWork.ProviderAvailabilities
.FindSingleAsync(a => a.Id == availabilityId && a.ServiceProviderId == profile.Id);

if (availability == null)
return _responseHandler.NotFound<string>("Availability not found");

_unitOfWork.ProviderAvailabilities.Delete(availability);
await _unitOfWork.CompleteAsync();

return _responseHandler.Success<string>(null, "Availability deleted successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error deleting availability");
return _responseHandler.ServerError<string>("Error deleting availability");
}
}

public async Task<Response<string>> AddBulkAvailabilityAsync(
BulkAvailabilityRequest request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<string>("User not authenticated");

var profile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (profile == null)
return _responseHandler.NotFound<string>("Profile not found");

var availabilityList = new List<ProviderAvailability>();
var currentDate = request.StartDate.Date;

while (currentDate <= request.EndDate.Date)
{
if (request.ExcludeDays != null && request.ExcludeDays.Contains(currentDate.DayOfWeek))
{
currentDate = currentDate.AddDays(1);
continue;
}

var existing = await _unitOfWork.ProviderAvailabilities
.GetByDateAsync(profile.Id, currentDate);

if (existing == null)
{
availabilityList.Add(new ProviderAvailability
{
ServiceProviderId = profile.Id,
Date = currentDate,
IsAvailable = request.IsAvailable,
AvailableShift = request.AvailableShift
});
}

currentDate = currentDate.AddDays(1);
}

if (availabilityList.Any())
{
await _unitOfWork.ProviderAvailabilities.AddRangeAsync(availabilityList);
await _unitOfWork.CompleteAsync();
}

return _responseHandler.Success<string>(null, $"{availabilityList.Count} availability records added successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error adding bulk availability");
return _responseHandler.ServerError<string>("Error adding bulk availability");
}
}
private string GetStatusText(ServiceProviderApplicationStatus status)
{
return status switch
{
ServiceProviderApplicationStatus.Pending => "Pending Review",
ServiceProviderApplicationStatus.Approved => "Approved",
ServiceProviderApplicationStatus.Rejected => "Rejected",
_ => "Unknown"
};
}
private string GetStatusMessage(ServiceProviderApplicationStatus status)
{
return status switch
{
ServiceProviderApplicationStatus.Pending => "Your application is currently under review.",
ServiceProviderApplicationStatus.Approved => "Your application has been approved. You can now access your provider dashboard.",
ServiceProviderApplicationStatus.Rejected => "Your application was rejected. You can check the rejection reason and reapply.",
_ => "Unknown status."
};
}

public async Task<Response<PaginatedResult<ProviderSummaryResponse>>> SearchProvidersAsync(GetProvidersRequest request)
{
try
{
var (items, totalCount) = await _unitOfWork.ServiceProviderProfiles.SearchProvidersAsync(
request.Available,
request.Governorate,
request.City,
request.CategoryId,
request.Search,
request.Page,
request.PageSize
);

var responses = items.Select(p => new ProviderSummaryResponse
{
Id = p.Id,
FullName = $"{p.User.FirstName} {p.User.LastName}",
Categories = p.Categories.Select(c => new CategoryDto
{
Id = c.Category.Id,
Name = c.Category.Name
}).ToList(),
Location = p.WorkingAreas.FirstOrDefault() != null
? new LocationDto
{
Governorate = p.WorkingAreas.First().Governorate,
City = p.WorkingAreas.First().City
}
: new LocationDto(),
IsAvailable = p.IsAvailable,
AverageRating = p.AverageRating,
HourlyRate = p.HourlyRate
}).ToList();
var paginatedResponse = new PaginatedResult<ProviderSummaryResponse>
{
Items = responses,
Page = request.Page,
PageSize = request.PageSize,
TotalCount = totalCount
};

return _responseHandler.Success(paginatedResponse, "Providers retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error searching providers");
return _responseHandler.ServerError<PaginatedResult<ProviderSummaryResponse>>("Error retrieving providers");
}
}

public async Task<Response<ProviderDetailResponse>> GetProviderDetailAsync(Guid providerId)
{
try
{
var provider = await _unitOfWork.ServiceProviderProfiles.GetProviderWithDetailsAsync(providerId);
if (provider == null)
return _responseHandler.NotFound<ProviderDetailResponse>("Provider not found");

var shiftPrices = new List<ShiftPriceDto>();
foreach (var providerCategory in provider.Categories)
{
var pricing = providerCategory.Category.Pricing.Where(p => p.IsActive);
foreach (var price in pricing)
{
shiftPrices.Add(new ShiftPriceDto
{
CategoryId = providerCategory.CategoryId,
CategoryName = providerCategory.Category.Name,
ShiftType = price.ShiftType,
ShiftTypeName = GetShiftTypeName(price.ShiftType),
PricePerShift = price.PricePerShift,
PricingId = price.Id
});
}
}

var response = new ProviderDetailResponse
{
Id = provider.Id,
FullName = $"{provider.User.FirstName} {provider.User.LastName}",

Bio = provider.Bio,
Categories = provider.Categories.Select(c => new CategoryDto
{
Id = c.Category.Id,
Name = c.Category.Name
}).ToList(),
WorkingAreas = provider.WorkingAreas.Select(w => new  ProviderWorkingAreaDto
{
Governorate = w.Governorate,
City = w.City,
District = w.District
}).ToList(),
Availability = provider.Availability
.OrderBy(a => a.Date)
.Take(30)
.Select(a => new ElAnis.Entities.DTO.Provider.AvailabilityDto
{
Date = a.Date,
IsAvailable = a.IsAvailable,
AvailableShift = a.AvailableShift,
ShiftName = a.AvailableShift.HasValue ? GetShiftTypeName(a.AvailableShift.Value) : null
}).ToList(),
ShiftPrices = shiftPrices,
AverageRating = provider.AverageRating,
TotalReviews = provider.TotalReviews,
HourlyRate = provider.HourlyRate,
IsAvailable = provider.IsAvailable
};

return _responseHandler.Success(response, "Provider details retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting provider details");
return _responseHandler.ServerError<ProviderDetailResponse>("Error retrieving provider details");
}
}

private string GetShiftTypeName(ShiftType shiftType)
{
return shiftType switch
{
ShiftType.ThreeHours => "3 Hours",
ShiftType.TwelveHours => "12 Hours",
ShiftType.TwentyFourHours => "24 Hours",
_ => "Unknown"
};
}

}
```

# IServiceRequestService.cs
```cs
﻿using ElAnis.Entities.DTO.ServiceRequest;
using ElAnis.Entities.Shared.Bases;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.ServiceRequest
{
public interface IServiceRequestService
{
Task<Response<ServiceRequestResponse>> CreateRequestAsync(CreateServiceRequestDto request, ClaimsPrincipal userClaims);
Task<Response<List<ServiceRequestResponse>>> GetUserRequestsAsync(ClaimsPrincipal userClaims);
Task<Response<List<ServiceRequestResponse>>> GetProviderRequestsAsync(Guid providerId);
Task<Response<ServiceRequestResponse>> RespondToRequestAsync(Guid requestId, ProviderResponseDto response, ClaimsPrincipal userClaims);
Task<Response<ServiceRequestResponse>> StartRequestAsync(Guid requestId, ClaimsPrincipal userClaims);
Task<Response<ServiceRequestResponse>> CompleteRequestAsync(Guid requestId, ClaimsPrincipal userClaims);
}
}
```

# ServiceRequestService.cs
```cs
﻿using ElAnis.Entities.DTO.ServiceRequest;
using ElAnis.Entities.Models;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.ServiceRequest
{
public class ServiceRequestService : IServiceRequestService
{
private readonly IUnitOfWork _unitOfWork;
private readonly ILogger<ServiceRequestService> _logger;
private readonly ResponseHandler _responseHandler;

public ServiceRequestService(
IUnitOfWork unitOfWork,
ILogger<ServiceRequestService> logger,
ResponseHandler responseHandler)
{
_unitOfWork = unitOfWork;
_logger = logger;
_responseHandler = responseHandler;
}

public async Task<Response<ServiceRequestResponse>> CreateRequestAsync(
CreateServiceRequestDto request,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ServiceRequestResponse>("User not authenticated");

var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
if (category == null)
return _responseHandler.NotFound<ServiceRequestResponse>("Category not found");

var pricing = await _unitOfWork.ServicePricings.GetByShiftTypeAsync(request.CategoryId, request.ShiftType);
if (pricing == null || !pricing.IsActive)
return _responseHandler.BadRequest<ServiceRequestResponse>("Pricing not available for selected category and shift");

if (request.ProviderId.HasValue)
{
var provider = await _unitOfWork.ServiceProviderProfiles.GetByIdAsync(request.ProviderId.Value);
if (provider == null || provider.Status != ServiceProviderStatus.Approved)
return _responseHandler.NotFound<ServiceRequestResponse>("Provider not found or not approved");

var isAvailable = await _unitOfWork.ServiceProviderProfiles
.IsProviderAvailableOnDateAsync(request.ProviderId.Value, request.PreferredDate, request.ShiftType);

if (!isAvailable)
return _responseHandler.BadRequest<ServiceRequestResponse>("Provider is not available on the selected date and shift");

var hasPending = await _unitOfWork.ServiceRequests
.HasPendingRequestAsync(userId, request.ProviderId.Value, request.PreferredDate);

if (hasPending)
return _responseHandler.BadRequest<ServiceRequestResponse>("You already have a pending request with this provider for this date");
}

var serviceRequest = new ElAnis.Entities.Models.ServiceRequest
{
UserId = userId,
ServiceProviderId = request.ProviderId,
CategoryId = request.CategoryId,
ShiftType = request.ShiftType,
TotalPrice = pricing.PricePerShift,  // Server-side price
PreferredDate = request.PreferredDate,
Address = request.Address,
Governorate = request.Governorate,
Description = request.Description ?? string.Empty,
Status = ServiceRequestStatus.Pending,
CreatedAt = DateTime.UtcNow
};

await _unitOfWork.ServiceRequests.AddAsync(serviceRequest);
await _unitOfWork.CompleteAsync();

var response = MapToResponse(serviceRequest, category, null, null);
return _responseHandler.Created(response, "Request created successfully. Waiting for provider confirmation.");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error creating service request");
return _responseHandler.ServerError<ServiceRequestResponse>("Error creating service request");
}
}

public async Task<Response<List<ServiceRequestResponse>>> GetUserRequestsAsync(ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<List<ServiceRequestResponse>>("User not authenticated");

var requests = await _unitOfWork.ServiceRequests.GetUserRequestsAsync(userId);
var responses = requests.Select(r => MapToResponse(
r,
r.Category,
$"{r.ServiceProvider?.User.FirstName} {r.ServiceProvider?.User.LastName}",
null // No avatar for now
)).ToList();

return _responseHandler.Success(responses, "User requests retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting user requests");
return _responseHandler.ServerError<List<ServiceRequestResponse>>("Error retrieving user requests");
}
}

public async Task<Response<List<ServiceRequestResponse>>> GetProviderRequestsAsync(Guid providerId)
{
try
{
var provider = await _unitOfWork.ServiceProviderProfiles.GetByIdAsync(providerId);
if (provider == null)
return _responseHandler.NotFound<List<ServiceRequestResponse>>("Provider not found");

var requests = await _unitOfWork.ServiceRequests.GetProviderRequestsAsync(providerId);
var responses = requests.Select(r => MapToResponse(
r,
r.Category,
r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "Unknown User",
null // No avatar for now
)).ToList();

return _responseHandler.Success(responses, "Provider requests retrieved successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error getting provider requests");
return _responseHandler.ServerError<List<ServiceRequestResponse>>("Error retrieving provider requests");
}
}

public async Task<Response<ServiceRequestResponse>> RespondToRequestAsync(
Guid requestId,
ProviderResponseDto response,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ServiceRequestResponse>("User not authenticated");

var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(requestId);
if (serviceRequest == null)
return _responseHandler.NotFound<ServiceRequestResponse>("Service request not found");

var providerProfile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (providerProfile == null || serviceRequest.ServiceProviderId != providerProfile.Id)
return _responseHandler.Forbidden<ServiceRequestResponse>("You are not authorized to respond to this request");

if (serviceRequest.Status != ServiceRequestStatus.Pending)
return _responseHandler.BadRequest<ServiceRequestResponse>($"Request is already {serviceRequest.Status}");

if (response.Status == ServiceRequestStatus.Accepted)
{
serviceRequest.Status = ServiceRequestStatus.Accepted;
serviceRequest.AcceptedAt = DateTime.UtcNow;

}
else if (response.Status == ServiceRequestStatus.Rejected)
{
serviceRequest.Status = ServiceRequestStatus.Rejected;
serviceRequest.Description += $"\n[Rejection Reason: {response.Reason}]";

}

_unitOfWork.ServiceRequests.Update(serviceRequest);
await _unitOfWork.CompleteAsync();

var mappedResponse = MapToResponse(
serviceRequest,
serviceRequest.Category,
$"{providerProfile.User.FirstName} {providerProfile.User.LastName}",
null
);

return _responseHandler.Success(mappedResponse, $"Request {response.Status} successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error responding to service request");
return _responseHandler.ServerError<ServiceRequestResponse>("Error processing provider response");
}
}

public async Task<Response<ServiceRequestResponse>> StartRequestAsync(
Guid requestId,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ServiceRequestResponse>("User not authenticated");

var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(requestId);
if (serviceRequest == null)
return _responseHandler.NotFound<ServiceRequestResponse>("Service request not found");

var providerProfile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (providerProfile == null)
return _responseHandler.Forbidden<ServiceRequestResponse>("Provider profile not found");

if (serviceRequest.ServiceProviderId != providerProfile.Id)
return _responseHandler.Forbidden<ServiceRequestResponse>("You are not authorized to start this request");

if (serviceRequest.Status != ServiceRequestStatus.Paid)
return _responseHandler.BadRequest<ServiceRequestResponse>(
$"Cannot start request. Current status is {serviceRequest.Status}. Request must be Paid first.");

serviceRequest.Status = ServiceRequestStatus.InProgress;
serviceRequest.StartedAt = DateTime.UtcNow;

_unitOfWork.ServiceRequests.Update(serviceRequest);
await _unitOfWork.CompleteAsync();

_logger.LogInformation(
"Service request {RequestId} started by provider {ProviderId} at {StartedAt}",
requestId, providerProfile.Id, serviceRequest.StartedAt);

var response = MapToResponse(
serviceRequest,
serviceRequest.Category,
$"{providerProfile.User.FirstName} {providerProfile.User.LastName}",
null
);

return _responseHandler.Success(response, "Service started successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error starting service request {RequestId}", requestId);
return _responseHandler.ServerError<ServiceRequestResponse>("Error starting service request");
}
}

public async Task<Response<ServiceRequestResponse>> CompleteRequestAsync(
Guid requestId,
ClaimsPrincipal userClaims)
{
try
{
var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
return _responseHandler.Unauthorized<ServiceRequestResponse>("User not authenticated");

var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(requestId);
if (serviceRequest == null)
return _responseHandler.NotFound<ServiceRequestResponse>("Service request not found");

var providerProfile = await _unitOfWork.ServiceProviderProfiles
.FindSingleAsync(p => p.UserId == userId);

if (providerProfile == null)
return _responseHandler.Forbidden<ServiceRequestResponse>("Provider profile not found");

if (serviceRequest.ServiceProviderId != providerProfile.Id)
return _responseHandler.Forbidden<ServiceRequestResponse>("You are not authorized to complete this request");

if (serviceRequest.Status != ServiceRequestStatus.InProgress)
return _responseHandler.BadRequest<ServiceRequestResponse>(
$"Cannot complete request. Current status is {serviceRequest.Status}. Request must be InProgress first.");

serviceRequest.Status = ServiceRequestStatus.Completed;
serviceRequest.CompletedAt = DateTime.UtcNow;

providerProfile.CompletedJobs++;
providerProfile.TotalEarnings += serviceRequest.TotalPrice;

if (serviceRequest.PreferredDate.Date != DateTime.UtcNow.Date)
{
providerProfile.WorkedDays++;
}

_unitOfWork.ServiceRequests.Update(serviceRequest);
_unitOfWork.ServiceProviderProfiles.Update(providerProfile);
await _unitOfWork.CompleteAsync();

_logger.LogInformation(
"Service request {RequestId} completed by provider {ProviderId} at {CompletedAt}. " +
"Provider stats updated: CompletedJobs={CompletedJobs}, TotalEarnings={TotalEarnings}",
requestId, providerProfile.Id, serviceRequest.CompletedAt,
providerProfile.CompletedJobs, providerProfile.TotalEarnings);

var response = MapToResponse(
serviceRequest,
serviceRequest.Category,
$"{providerProfile.User.FirstName} {providerProfile.User.LastName}",
null
);

return _responseHandler.Success(response, "Service completed successfully");
}
catch (Exception ex)
{
_logger.LogError(ex, "Error completing service request {RequestId}", requestId);
return _responseHandler.ServerError<ServiceRequestResponse>("Error completing service request");
}
}

private ServiceRequestResponse MapToResponse(
ElAnis.Entities.Models.ServiceRequest request,
ElAnis.Entities.Models.Category category,
string? providerName,
string? providerAvatar
)
{
return new ServiceRequestResponse
{
Id = request.Id,
ProviderId = request.ServiceProviderId,
ProviderName = providerName,
ProviderAvatar = providerAvatar,
CategoryId = request.CategoryId,
CategoryName = category.Name,
Status = request.Status,
StatusName = request.Status.ToString(),
TotalPrice = request.TotalPrice,
PreferredDate = request.PreferredDate,
ShiftType = request.ShiftType,
ShiftTypeName = GetShiftTypeName(request.ShiftType),
Address = request.Address,
Description = request.Description,
CreatedAt = request.CreatedAt,
AcceptedAt = request.AcceptedAt,
StartedAt = request.StartedAt,        // ✅ Add
CompletedAt = request.CompletedAt,    // ✅ Add
CanPay = request.Status == ServiceRequestStatus.Accepted && request.Payment == null,
CanStart = request.Status == ServiceRequestStatus.Paid,           // ✅ Add
CanComplete = request.Status == ServiceRequestStatus.InProgress   // ✅ Add
};
}

private string GetShiftTypeName(ShiftType shiftType)
{
return shiftType switch
{
ShiftType.ThreeHours => "3 Hours",
ShiftType.TwelveHours => "12 Hours",
ShiftType.TwentyFourHours => "24 Hours",
_ => "Unknown"
};
}
}
}
```

# ITokenStoreService.cs
```cs
﻿using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.DataAccess.Services.Token
{
public interface ITokenStoreService
{
Task<string> CreateAccessTokenAsync(User appUser);
string GenerateRefreshToken();
Task SaveRefreshTokenAsync(string userId, string refreshToken);
Task InvalidateOldTokensAsync(string userId);
Task<bool> IsValidAsync(string refreshToken);
Task<(string AccessToken, string RefreshToken)> GenerateAndStoreTokensAsync(string userId, User user);

}
}
```

# TokenStoreService.cs
```cs
﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using ElAnis.DataAccess.ApplicationContext;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models.Auth.UserTokens;
using ElAnis.Utilities.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ElAnis.DataAccess.Services.Token
{
public class TokenStoreService : ITokenStoreService
{
private readonly SymmetricSecurityKey _symmetricSecurityKey;
private readonly UserManager<User> _userManager;
private readonly JwtSettings _jwtSettings;
private readonly IUnitOfWork _unitOfWork;

public TokenStoreService(
IOptions<JwtSettings> jwtOptions,
UserManager<User> userManager,
IUnitOfWork unitOfWork)
{
_jwtSettings = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
_userManager = userManager;
_unitOfWork = unitOfWork;

if (string.IsNullOrEmpty(_jwtSettings.SigningKey))
{
throw new ArgumentException("JWT SigningKey is not configured.");
}
_symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
}

public async Task<string> CreateAccessTokenAsync(User appUser)
{
var roles = await _userManager.GetRolesAsync(appUser);
var claims = new List<Claim>
{
new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
new Claim(ClaimTypes.Email, appUser.Email ?? ""),
new Claim(ClaimTypes.GivenName, appUser.UserName ?? ""),
new Claim("UserId", appUser.Id.ToString()),
new Claim("FullName", $"{appUser.FirstName} {appUser.LastName}".Trim())
};

foreach (var role in roles)
{
claims.Add(new Claim(ClaimTypes.Role, role));
}

if (roles.Contains("Provider") || roles.Contains("PROVIDER"))
{
var serviceProvider = await _unitOfWork.ServiceProviderProfiles
.GetByUserIdAsync(appUser.Id);

if (serviceProvider != null)
{
claims.Add(new Claim("ServiceProviderId", serviceProvider.Id.ToString()));
claims.Add(new Claim("ServiceProviderStatus", serviceProvider.Status.ToString()));
claims.Add(new Claim("IsAvailable", serviceProvider.IsAvailable.ToString()));

Console.WriteLine($"✅ [Token] Provider Profile Found - Status: {serviceProvider.Status}");
}
else
{
var application = await _unitOfWork.ServiceProviderApplications
.FindSingleAsync(a => a.UserId == appUser.Id);

if (application != null)
{
claims.Add(new Claim("ApplicationId", application.Id.ToString()));
claims.Add(new Claim("ServiceProviderStatus", application.Status.ToString()));

Console.WriteLine($"🟡 [Token] Application Found - Status: {application.Status}");
}
else
{
Console.WriteLine($"⚠️ [Token] No Profile or Application found for user {appUser.Id}");
}
}
}

var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
var tokenDescriptor = new SecurityTokenDescriptor
{
Subject = new ClaimsIdentity(claims),
Expires = DateTime.Now.AddDays(7),
SigningCredentials = creds,
Issuer = _jwtSettings.Issuer,
Audience = _jwtSettings.Audience,
};

var tokenHandler = new JwtSecurityTokenHandler();
var token = tokenHandler.CreateToken(tokenDescriptor);

var tokenString = tokenHandler.WriteToken(token);

Console.WriteLine($"🎫 [Token Generated] User: {appUser.Email}, Roles: {string.Join(", ", roles)}");
var statusClaim = claims.FirstOrDefault(c => c.Type == "ServiceProviderStatus");
if (statusClaim != null)
{
Console.WriteLine($"   └─ ServiceProviderStatus: {statusClaim.Value}");
}

return tokenString;
}

public string GenerateRefreshToken()
{
return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}

public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
{
var userRefreshTokenRepo = _unitOfWork.Repository<UserRefreshToken>();

await userRefreshTokenRepo.AddAsync(new UserRefreshToken
{
UserId = userId,
Token = refreshToken,
ExpiryDateUtc = DateTime.UtcNow.AddDays(7),
IsUsed = false
});

await _unitOfWork.CompleteAsync();
}

public async Task InvalidateOldTokensAsync(string userId)
{
var userRefreshTokenRepo = _unitOfWork.Repository<UserRefreshToken>();

var tokens = await userRefreshTokenRepo.FindAsync(r => r.UserId == userId);

if (tokens.Any())
{
userRefreshTokenRepo.DeleteRange(tokens);
await _unitOfWork.CompleteAsync();
}
}

public async Task<bool> IsValidAsync(string refreshToken)
{
var userRefreshTokenRepo = _unitOfWork.Repository<UserRefreshToken>();

return await userRefreshTokenRepo.AnyAsync(r =>
r.Token == refreshToken &&
!r.IsUsed &&
r.ExpiryDateUtc > DateTime.UtcNow);
}

public async Task<(string AccessToken, string RefreshToken)> GenerateAndStoreTokensAsync(string userId, User user)
{
var accessToken = await CreateAccessTokenAsync(user);
var refreshToken = GenerateRefreshToken();
await SaveRefreshTokenAsync(userId, refreshToken);
return (accessToken, refreshToken);
}
}
}
```

# Category.cs
```cs
﻿using System;

namespace ElAnis.Entities.Models
{
public class Category
{
public Guid Id { get; set; }

public string Name { get; set; } = string.Empty; // "Elderly Care - Standard"
public string Description { get; set; } = string.Empty;
public string Icon { get; set; } = string.Empty;
public bool IsActive { get; set; } = true;
public int DisplayOrder { get; set; } = 0; // ترتيب العرض
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

public ICollection<ServicePricing> Pricing { get; set; } = new List<ServicePricing>();
public ICollection<ServiceProviderCategory> ServiceProviders { get; set; } = new List<ServiceProviderCategory>();
public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
}
```

# Notification.cs
```cs
﻿// Notification.cs - للإشعارات
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;
using ElAnis.Utilities.Enum.ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class Notification
{
public Guid Id { get; set; }
public string UserId { get; set; } = null!;
public User User { get; set; } = null!;

public string Title { get; set; } = string.Empty;
public string Message { get; set; } = string.Empty;
public NotificationType Type { get; set; }
public bool IsRead { get; set; } = false;

public Guid? ServiceRequestId { get; set; }  // بدل int?
public ServiceRequest? ServiceRequest { get; set; }  // خلي nullable لو FK اختياري

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
}
```

# Payment.cs
```cs
﻿using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class Payment
{
public Guid Id { get; set; }

public Guid ServiceRequestId { get; set; }
public ServiceRequest ServiceRequest { get; set; } = null!;

public decimal Amount { get; set; }
public PaymentMethod PaymentMethod { get; set; } // كاش، فيزا، فودافون كاش، إلخ
public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

public string? TransactionId { get; set; } // رقم المعاملة من payment gateway
public string? PaymentGatewayResponse { get; set; } // JSON response

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? PaidAt { get; set; }
}
}
```

# ProviderAvailability.cs
```cs
﻿using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class ProviderAvailability
{
public Guid Id { get; set; }
public Guid ServiceProviderId { get; set; }
public ServiceProviderProfile ServiceProvider { get; set; } = null!;

public DateTime Date { get; set; } // التاريخ
public bool IsAvailable { get; set; } = true; // متاح أم لا
public ShiftType? AvailableShift { get; set; } // الشيفت المتاح (إذا كان متاح)

public string? Notes { get; set; }

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? UpdatedAt { get; set; }
}
}
```

# ProviderWorkingArea.cs
```cs
﻿namespace ElAnis.Entities.Models
{
public class ProviderWorkingArea
{
public Guid Id { get; set; }
public Guid ServiceProviderId { get; set; }
public ServiceProviderProfile ServiceProvider { get; set; } = null!;

public string Governorate { get; set; } = string.Empty; // القاهرة، الجيزة، الإسكندرية
public string? City { get; set; } // المدينة داخل المحافظة
public string? District { get; set; } // الحي

public bool IsActive { get; set; } = true;
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
}
```

# Review.cs
```cs
﻿using ElAnis.Entities.Models.Auth.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElAnis.Entities.Models
{
public class Review
{
public Guid Id { get; set; }

public string ClientUserId { get; set; } = null!;
public User Client { get; set; } = null!;

public string ServiceProviderUserId { get; set; } = null!;
public User ServiceProvider { get; set; } = null!;
public Guid ServiceRequestId { get; set; } // بدل int
public ServiceRequest ServiceRequest { get; set; } = null!;

[Range(1, 5)]
public int Rating { get; set; } // 1-5

[MaxLength(1000)]
public string? Comment { get; set; }

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
}
```

# ServicePricing.cs
```cs
﻿// ===== 3. ServicePricing.cs (Updated) =====
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class ServicePricing
{
public Guid Id { get; set; }

public Guid CategoryId { get; set; }
public Category Category { get; set; } = null!;

public ShiftType ShiftType { get; set; }

public decimal PricePerShift { get; set; }

public string? Description { get; set; }
public bool IsActive { get; set; } = true;

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? UpdatedAt { get; set; }

public string? UpdatedBy { get; set; }
}
}
```

# ServiceProviderApplication.cs
```cs
﻿using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class ServiceProviderApplication
{
public Guid Id { get; set; } = Guid.NewGuid();
public string UserId { get; set; } = null!;
public User User { get; set; } = null!;

public string FirstName { get; set; } = string.Empty;
public string LastName { get; set; } = string.Empty;
public string PhoneNumber { get; set; } = string.Empty;
public string Address { get; set; } = string.Empty;
public DateTime DateOfBirth { get; set; }

public string Bio { get; set; } = string.Empty;
public string NationalId { get; set; } = string.Empty;
public string Experience { get; set; } = string.Empty;
public decimal HourlyRate { get; set; }///

public string IdDocumentPath { get; set; } = string.Empty;
public string CertificatePath { get; set; } = string.Empty;
public string CVPath { get; set; } = string.Empty;

public List<Guid> SelectedCategories { get; set; } = new();

public ServiceProviderApplicationStatus Status { get; set; } = ServiceProviderApplicationStatus.Pending;
public string? RejectionReason { get; set; }
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? ReviewedAt { get; set; }
public string? ReviewedById { get; set; }
public User? ReviewedBy { get; set; }
}
}
```

# ServiceProviderCategory.cs
```cs
﻿using System;

namespace ElAnis.Entities.Models
{

public class ServiceProviderCategory
{
public Guid ServiceProviderId { get; set; }
public ServiceProviderProfile ServiceProvider { get; set; } = null!;

public Guid CategoryId { get; set; }
public Category Category { get; set; } = null!;

public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

}
```

# ServiceProviderProfile.cs
```cs
﻿using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class ServiceProviderProfile
{
public Guid Id { get; set; }
public string UserId { get; set; } = null!;
public User User { get; set; } = null!;

public string? Bio { get; set; }
public string? NationalId { get; set; }
public string? Experience { get; set; }
public decimal HourlyRate { get; set; }
public string? IdDocumentPath { get; set; }
public string? CertificatePath { get; set; }
public string CVPath { get; set; } = string.Empty;

public int CompletedJobs { get; set; } = 0;
public decimal TotalEarnings { get; set; } = 0;
public double AverageRating { get; set; } = 0;
public int TotalReviews { get; set; } = 0;
public int WorkedDays { get; set; } = 0;

public ServiceProviderStatus Status { get; set; } = ServiceProviderStatus.Pending;
public bool IsAvailable { get; set; } = true;
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? ApprovedAt { get; set; }
public string? RejectionReason { get; set; }

public ICollection<ServiceProviderCategory> Categories { get; set; } = new List<ServiceProviderCategory>();
public ICollection<ProviderWorkingArea> WorkingAreas { get; set; } = new List<ProviderWorkingArea>();
public ICollection<ProviderAvailability> Availability { get; set; } = new List<ProviderAvailability>();
public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
}
}
```

# ServiceRequest.cs
```cs
﻿using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Utilities.Enum;

namespace ElAnis.Entities.Models
{
public class ServiceRequest
{
public Guid Id { get; set; }
public string UserId { get; set; } = null!;
public User User { get; set; } = null!;

public Guid ? ServiceProviderId { get; set; }
public ServiceProviderProfile? ServiceProvider { get; set; }

public Guid CategoryId { get; set; }
public Category Category { get; set; } = null!;

public string Description { get; set; } = string.Empty;

public string Governorate { get; set; } = string.Empty; // المحافظة

public string Address { get; set; } = string.Empty;
public DateTime PreferredDate { get; set; }

public decimal TotalPrice { get; set; }

public ShiftType ShiftType { get; set; } // نوع الشيفت (3ساعات، 12ساعة، 24ساعة)
public decimal? OfferedPrice { get; set; }

public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime? AcceptedAt { get; set; } // وقت قبول البروفايدر
public DateTime? StartedAt { get; set; } // وقت بدء الخدمة
public DateTime? CompletedAt { get; set; } // وقت انتهاء الخدمة

public Payment? Payment { get; set; }
public Review? Review { get; set; }
}
}
```

# IResponse.cs
```cs
﻿namespace ElAnis.Entities.Shared.Bases
{
public interface IResponse
{
}
}
```

# Response.cs
```cs
﻿using System.Net;

namespace ElAnis.Entities.Shared.Bases
{
public class Response<T>:IResponse
{
public Response()
{

}
public Response(T data, string message = null)
{
Succeeded = true;
Message = message;
Data = data;
}
public Response(string message)
{
Succeeded = false;
Message = message;
}
public Response(string message, bool succeeded)
{
Succeeded = succeeded;
Message = message;
}

public HttpStatusCode StatusCode { get; set; }
public bool Succeeded { get; set; }
public string Message { get; set; }
public List<string> Errors { get; set; }
public T Data { get; set; }
}
}
```

# ResponseHandler.cs
```cs
﻿using Azure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ElAnis.Entities.Shared.Bases
{
public class ResponseHandler
{
public Response<T> Deleted<T>(string Message = null)
{
return new Response<T>()
{
StatusCode = System.Net.HttpStatusCode.OK,
Succeeded = true,
Message = Message
};
}
public Response<T> Success<T>(T entity, string message)
{
return new Response<T>()
{
Data = entity,
StatusCode = System.Net.HttpStatusCode.OK,
Succeeded = true,
Message = message
};
}
public Response<T> Unauthorized<T>(string Message = null)
{
return new Response<T>()
{
StatusCode = System.Net.HttpStatusCode.Unauthorized,
Succeeded = true,
Message = Message
};
}
public Response<T> BadRequest<T>(string Message = null)
{
return new Response<T>()
{
StatusCode = System.Net.HttpStatusCode.BadRequest,
Succeeded = false,
Message = Message
};
}
public Response<T> UnprocessableEntity<T>(string Message = null)
{
return new Response<T>()
{
StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
Succeeded = false,
Message = Message
};
}
public Response<T> NotFound<T>(string message = null)
{
return new Response<T>()
{
StatusCode = System.Net.HttpStatusCode.NotFound,
Succeeded = false,
Message = message
};
}
public Response<T> Created<T>(T entity, string message = null)
{
return new Response<T>()
{
Data = entity,
StatusCode = System.Net.HttpStatusCode.Created,
Succeeded = true,
Message = message
};
}
public Response<T> ServerError<T>(string message = "An unexpected error occurred.")
{
return new Response<T>
{
StatusCode = HttpStatusCode.InternalServerError,
Succeeded = false,
Message = message,
};
}
public Response<T> InternalServerError<T>(string message = null)
{
return new Response<T>()
{
StatusCode = System.Net.HttpStatusCode.InternalServerError,
Succeeded = false,
Message = message
};
}
public Response<T> Forbidden<T>(string message)
{
return new Response<T>
{
StatusCode = System.Net.HttpStatusCode.Forbidden,
Succeeded = false,
Message = message,
Errors = new List<string> { message }
};
}
public IActionResult HandleModelStateErrors(ModelStateDictionary modelState)
{
var errors = modelState.Values.SelectMany(v => v.Errors)
.Select(e => e.ErrorMessage)
.ToList();
return new BadRequestObjectResult(new Response<object>
{
StatusCode = HttpStatusCode.BadRequest,
Succeeded = false,
Errors = errors,
Message = "There is an error while operation please try again"
});
}
}
}
```

# AccountController.cs
```cs
﻿using ElAnis.DataAccess.Services.Auth;
using ElAnis.DataAccess.Services.OAuth;
using ElAnis.Entities.DTO.Account.Auth;
using ElAnis.Entities.DTO.Account.Auth.Login;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Account.Auth.ResetPassword;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
private readonly IAuthService _authService;
private readonly ResponseHandler _responseHandler;
private readonly IValidator<RegisterRequest> _registerValidator;
private readonly IValidator<RegisterServiceProviderRequest> _serviceProviderRegisterValidator;
private readonly IValidator<AdminRegisterRequest> _adminRegisterValidator;
private readonly IValidator<LoginRequest> _loginValidator;
private readonly IValidator<ForgetPasswordRequest> _forgetPasswordValidator;
private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
private readonly IAuthGoogleService _authGoogleService;

public AccountController(
IAuthService authService,
ResponseHandler responseHandler,
IValidator<RegisterRequest> registerValidator,
IValidator<RegisterServiceProviderRequest> serviceProviderRegisterValidator,
IValidator<AdminRegisterRequest> adminRegisterValidator,
IValidator<LoginRequest> loginValidator,
IValidator<ForgetPasswordRequest> forgetPasswordValidator,
IValidator<ResetPasswordRequest> resetPasswordValidator,
IAuthGoogleService authGoogleService,
IValidator<ChangePasswordRequest> changePasswordValidator)
{
_authService = authService;
_responseHandler = responseHandler;
_registerValidator = registerValidator;
_serviceProviderRegisterValidator = serviceProviderRegisterValidator;
_adminRegisterValidator = adminRegisterValidator;
_loginValidator = loginValidator;
_forgetPasswordValidator = forgetPasswordValidator;
_resetPasswordValidator = resetPasswordValidator;
_authGoogleService = authGoogleService;
_changePasswordValidator = changePasswordValidator;
}

[HttpPost("login")]
[ProducesResponseType(typeof(Response<LoginResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<LoginResponse>>> Login([FromBody] LoginRequest request)
{
ValidationResult validationResult = await _loginValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.LoginAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("login/google")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleLoginDto)
{
if (!ModelState.IsValid)
return _responseHandler.HandleModelStateErrors(ModelState);

try
{
var token = await _authGoogleService.AuthenticateWithGoogleAsync(googleLoginDto.IdToken);
var response = _responseHandler.Success(token, "Logged in with Google successfully");
return StatusCode((int)response.StatusCode, response);
}
catch (UnauthorizedAccessException ex)
{
var response = _responseHandler.Unauthorized<string>("Google authentication failed: " + ex.Message);
return StatusCode((int)response.StatusCode, response);
}
catch (UserCreationException ex)
{
var response = _responseHandler.InternalServerError<string>("User creation failed: " + ex.Message);
return StatusCode((int)response.StatusCode, response);
}
catch (Exception ex)
{
var response = _responseHandler.ServerError<string>("An error occurred: " + ex.Message);
return StatusCode((int)response.StatusCode, response);
}
}

[HttpPost("register-user")]
[ProducesResponseType(typeof(Response<RegisterResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<RegisterResponse>>> RegisterUser([FromForm] RegisterRequest request)
{
ValidationResult validationResult = await _registerValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.RegisterUserAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("register-service-provider")]
[ProducesResponseType(typeof(Response<ServiceProviderApplicationResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<ServiceProviderApplicationResponse>>> RegisterServiceProvider([FromForm] RegisterServiceProviderRequest request)
{
ValidationResult validationResult = await _serviceProviderRegisterValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.RegisterServiceProviderAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("create-admin")]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<RegisterResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<RegisterResponse>>> CreateAdmin([FromBody] AdminRegisterRequest request)
{
ValidationResult validationResult = await _adminRegisterValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.CreateAdminAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("verify-otp")]
[ProducesResponseType(typeof(Response<bool>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<bool>>> VerifyOtp([FromBody] VerifyOtpRequest model)
{
if (!ModelState.IsValid)
return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
_responseHandler.BadRequest<object>("Invalid input data."));

var result = await _authService.VerifyOtpAsync(model);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("resend-otp")]
[EnableRateLimiting("SendOtpPolicy")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 429)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<string>>> ResendOtp([FromBody] ResendOtpRequest model)
{
if (!ModelState.IsValid)
return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
_responseHandler.BadRequest<object>("Invalid input data."));

var result = await _authService.ResendOtpAsync(model);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("forget-password")]
[ProducesResponseType(typeof(Response<ForgetPasswordResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<ForgetPasswordResponse>>> ForgetPassword([FromBody] ForgetPasswordRequest request)
{
ValidationResult validationResult = await _forgetPasswordValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.ForgotPasswordAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("reset-password")]
[ProducesResponseType(typeof(Response<ResetPasswordResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<ActionResult<Response<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
{
ValidationResult validationResult = await _resetPasswordValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.ResetPasswordAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("refresh-token")]
[ProducesResponseType(typeof(Response<RefreshTokenResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
{
if (string.IsNullOrWhiteSpace(refreshToken))
return BadRequest(_responseHandler.BadRequest<string>("RefreshTokenIsNotFound"));
try
{
var newTokens = await _authService.RefreshTokenAsync(refreshToken);
return Ok(_responseHandler.Success<RefreshTokenResponse>(newTokens.Data, "User token refreshed successfully"));
}
catch (SecurityTokenException ex)
{
return Unauthorized(_responseHandler.Unauthorized<string>(ex.Message));
}
catch (Exception ex)
{
var error = ex.InnerException?.Message ?? ex.Message;
return StatusCode(
StatusCodes.Status500InternalServerError,
_responseHandler.BadRequest<string>("UnexpectedError" + ": " + error)
);
}
}

[HttpPost("change-password")]
[Authorize]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
{
var validationResult = await _changePasswordValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _authService.ChangePasswordAsync(User, request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("logout")]
[Authorize]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> Logout()
{
var response = await _authService.LogoutAsync(User);
return StatusCode((int)response.StatusCode, response);
}
}
}
```

# AdminController.cs
```cs
﻿using ElAnis.DataAccess.Services.Admin;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
private readonly IAdminService _adminService;
private readonly ResponseHandler _responseHandler;
private readonly IValidator<RejectApplicationRequest> _rejectApplicationValidator;
private readonly IValidator<SuspendServiceProviderRequest> _suspendServiceProviderValidator;

public AdminController(
IAdminService adminService,
ResponseHandler responseHandler,
IValidator<RejectApplicationRequest> rejectApplicationValidator,
IValidator<SuspendServiceProviderRequest> suspendServiceProviderValidator)
{
_adminService = adminService;
_responseHandler = responseHandler;
_rejectApplicationValidator = rejectApplicationValidator;
_suspendServiceProviderValidator = suspendServiceProviderValidator;
}

[HttpGet("dashboard-stats")]
[ProducesResponseType(typeof(Response<AdminDashboardStatsDto>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetDashboardStats()
{
var response = await _adminService.GetDashboardStatsAsync();
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("service-provider-applications")]
[ProducesResponseType(typeof(Response<PaginatedResult<ServiceProviderApplicationDto>>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetServiceProviderApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
if (page < 1) page = 1;
if (pageSize < 1 || pageSize > 100) pageSize = 10;

var response = await _adminService.GetServiceProviderApplicationsAsync(page, pageSize);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("service-provider-applications/{id}")]
[ProducesResponseType(typeof(Response<ServiceProviderApplicationDetailDto>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetServiceProviderApplicationById(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

var response = await _adminService.GetServiceProviderApplicationByIdAsync(id);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("service-provider-applications/{id}/approve")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> ApproveServiceProviderApplication(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

var response = await _adminService.ApproveServiceProviderApplicationAsync(id, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("service-provider-applications/{id}/reject")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> RejectServiceProviderApplication(Guid id, [FromBody] RejectApplicationRequest request)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

ValidationResult validationResult = await _rejectApplicationValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _adminService.RejectServiceProviderApplicationAsync(id, request.RejectionReason, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("service-providers/{id}/suspend")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 409)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> SuspendServiceProvider(Guid id, [FromBody] SuspendServiceProviderRequest request)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid service provider ID"));

ValidationResult validationResult = await _suspendServiceProviderValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _adminService.SuspendServiceProviderAsync(id, request.Reason, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("service-providers/{id}/activate")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 409)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> ActivateServiceProvider(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid service provider ID"));

var response = await _adminService.ActivateServiceProviderAsync(id, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("users")]
[ProducesResponseType(typeof(Response<PaginatedResult<UserManagementDto>>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
{
if (request.Page < 1) request.Page = 1;
if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

var response = await _adminService.GetUsersAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("users/{userId}/suspend")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> SuspendUser(string userId)
{
if (string.IsNullOrWhiteSpace(userId))
return BadRequest(_responseHandler.BadRequest<object>("Invalid user ID"));

var response = await _adminService.SuspendUserAsync(userId);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("users/{userId}/activate")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> ActivateUser(string userId)
{
if (string.IsNullOrWhiteSpace(userId))
return BadRequest(_responseHandler.BadRequest<object>("Invalid user ID"));

var response = await _adminService.ActivateUserAsync(userId);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("bookings/recent")]
[ProducesResponseType(typeof(Response<List<RecentBookingDto>>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetRecentBookings([FromQuery] int limit = 10)
{
if (limit < 1 || limit > 100) limit = 10;

var response = await _adminService.GetRecentBookingsAsync(limit);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("payments")]
[ProducesResponseType(typeof(Response<PaymentSummaryResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetPaymentTransactions()
{
var response = await _adminService.GetPaymentTransactionsAsync();
return StatusCode((int)response.StatusCode, response);
}
}
}
```

# CategoryController.cs
```cs
﻿using ElAnis.DataAccess.Services.Category;
using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
private readonly ICategoryService _categoryService;
private readonly ResponseHandler _responseHandler;
private readonly IValidator<CreateCategoryRequest> _createCategoryValidator;
private readonly IValidator<CreateCategoryRequest> _updateCategoryValidator;

public CategoryController(
ICategoryService categoryService,
ResponseHandler responseHandler,
IValidator<CreateCategoryRequest> createCategoryValidator,
IValidator<CreateCategoryRequest> updateCategoryValidator = null)
{
_categoryService = categoryService;
_responseHandler = responseHandler;
_createCategoryValidator = createCategoryValidator;
_updateCategoryValidator = updateCategoryValidator ?? createCategoryValidator;
}

[HttpGet]
[ProducesResponseType(typeof(Response<IEnumerable<CategoryDtoResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetAllCategories()
{
var response = await _categoryService.GetAllCategoriesAsync();
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("active")]
[ProducesResponseType(typeof(Response<IEnumerable<CategoryDtoResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetActiveCategories()
{
var response = await _categoryService.GetActiveCategoriesAsync();
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("{id}")]
[ProducesResponseType(typeof(Response<CategoryDtoResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetCategoryById(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

var response = await _categoryService.GetCategoryByIdAsync(id);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<CategoryDtoResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
{
if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _createCategoryValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _categoryService.CreateCategoryAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpPut("{id}")]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<CategoryDtoResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateCategoryRequest request)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _updateCategoryValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _categoryService.UpdateCategoryAsync(id, request);
return StatusCode((int)response.StatusCode, response);
}

[HttpDelete("{id}")]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 409)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> DeleteCategory(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

var response = await _categoryService.DeleteCategoryAsync(id);
return StatusCode((int)response.StatusCode, response);
}
}
}
```

# PaymentsController.cs
```cs
﻿using ElAnis.DataAccess.Services.Payment;
using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/payments")]
[ApiController]
public class PaymentsController : ControllerBase
{
private readonly IPaymentService _paymentService;
private readonly ResponseHandler _responseHandler;

public PaymentsController(
IPaymentService paymentService,
ResponseHandler responseHandler)
{
_paymentService = paymentService;
_responseHandler = responseHandler;
}

[HttpPost("create-checkout")]
[Authorize]
[ProducesResponseType(typeof(Response<PaymentResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> CreateCheckoutSession([FromBody] CreatePaymentDto request)
{
if (request.ServiceRequestId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid service request ID"));

var response = await _paymentService.CreateStripeCheckoutSessionAsync(request, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("webhook")]
[AllowAnonymous]
public async Task<IActionResult> StripeWebhook()
{
var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
var signature = Request.Headers["Stripe-Signature"].ToString();

if (string.IsNullOrEmpty(signature))
return BadRequest("Missing Stripe signature");

var response = await _paymentService.HandleStripeWebhookAsync(json, signature);

if (response.Succeeded)
return Ok();
else
return BadRequest(response.Message);
}

[HttpGet("request/{requestId}")]
[Authorize]
[ProducesResponseType(typeof(Response<PaymentResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetPaymentByRequestId(Guid requestId)
{
if (requestId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

var response = await _paymentService.GetPaymentByRequestIdAsync(requestId);
return StatusCode((int)response.StatusCode, response);
}
}
}
```

# ProviderController.cs
```cs
﻿using ElAnis.DataAccess.Services.ServiceProvider;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.DTO.Availability;
using ElAnis.Entities.DTO.Provider;
using ElAnis.Entities.DTO.ServiceProviderProfile;
using ElAnis.Entities.DTO.WorkingArea;
using ElAnis.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class ProviderController : ControllerBase
{
private readonly IServiceProviderService _providerService;
private readonly ResponseHandler _responseHandler;

public ProviderController(IServiceProviderService providerService, ResponseHandler responseHandler)
{
_providerService = providerService;
_responseHandler = responseHandler;
}

[HttpGet("application-status")]
[Authorize] // ✅ أي حد معمول login يقدر يشوف حالة طلبه
[ProducesResponseType(typeof(Response<ApplicationStatusResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetApplicationStatus()
{
var result = await _providerService.GetApplicationStatusAsync(User);
return StatusCode((int)result.StatusCode, result);
}

[HttpGet("dashboard")]
[Authorize(Roles = "Provider")] // ✅ تم التبسيط: بنتحقق من الـ Role فقط
[ProducesResponseType(typeof(Response<ProviderDashboardResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetDashboard()
{
var result = await _providerService.GetDashboardAsync(User);
return StatusCode((int)result.StatusCode, result);
}

[HttpGet("profile")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<ProviderProfileResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetProfile()
{
var result = await _providerService.GetProfileAsync(User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPut("profile")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<ProviderProfileResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> UpdateProfile([FromForm] UpdateProviderProfileRequest request)
{
var result = await _providerService.UpdateProfileAsync(request, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpGet]
[ProducesResponseType(typeof(Response<PaginatedResult<ProviderSummaryResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> SearchProviders([FromQuery] GetProvidersRequest request)
{
if (request.Page < 1)
return BadRequest(_responseHandler.BadRequest<object>("Page must be greater than 0"));

if (request.PageSize < 1 || request.PageSize > 100)
return BadRequest(_responseHandler.BadRequest<object>("PageSize must be between 1 and 100"));

var response = await _providerService.SearchProvidersAsync(request);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("{providerId}")]
[ProducesResponseType(typeof(Response<ProviderDetailResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetProviderDetail(Guid providerId)
{
if (providerId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid provider ID"));

var response = await _providerService.GetProviderDetailAsync(providerId);
return StatusCode((int)response.StatusCode, response);
}

[HttpPut("profile/availability")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> ToggleAvailability([FromBody] ToggleAvailabilityRequest request)
{
var result = await _providerService.ToggleAvailabilityAsync(request, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpGet("working-areas")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> GetWorkingAreas()
{
var result = await _providerService.GetWorkingAreasAsync(User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("working-areas")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> AddWorkingArea([FromBody] AddWorkingAreaRequest request)
{
var result = await _providerService.AddWorkingAreaAsync(request, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpDelete("working-areas/{id}")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> DeleteWorkingArea(Guid id)
{
var result = await _providerService.DeleteWorkingAreaAsync(id, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpGet("availability")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> GetAvailabilityCalendar([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
{
var start = startDate ?? DateTime.Today;
var end = endDate ?? DateTime.Today.AddMonths(1);
var result = await _providerService.GetAvailabilityCalendarAsync(start, end, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("availability")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityRequest request)
{
var result = await _providerService.AddAvailabilityAsync(request, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPut("availability/{id}")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> UpdateAvailability(Guid id, [FromBody] UpdateAvailabilityRequest request)
{
request.Id = id;
var result = await _providerService.UpdateAvailabilityAsync(request, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpDelete("availability/{id}")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> DeleteAvailability(Guid id)
{
var result = await _providerService.DeleteAvailabilityAsync(id, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("availability/bulk")]
[Authorize(Roles = "Provider")]
public async Task<IActionResult> AddBulkAvailability([FromBody] BulkAvailabilityRequest request)
{
var result = await _providerService.AddBulkAvailabilityAsync(request, User);
return StatusCode((int)result.StatusCode, result);
}
}
}
```

# RequestsController.cs
```cs
﻿using ElAnis.DataAccess.Services.ServiceRequest;
using ElAnis.Entities.DTO.ServiceRequest;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RequestsController : ControllerBase
{
private readonly IServiceRequestService _requestService;
private readonly ResponseHandler _responseHandler;
private readonly IValidator<CreateServiceRequestDto> _createValidator;
private readonly IValidator<ProviderResponseDto> _responseValidator;

public RequestsController(
IServiceRequestService requestService,
ResponseHandler responseHandler,
IValidator<CreateServiceRequestDto> createValidator,
IValidator<ProviderResponseDto> responseValidator)
{
_requestService = requestService;
_responseHandler = responseHandler;
_createValidator = createValidator;
_responseValidator = responseValidator;
}

[HttpPost]
[ProducesResponseType(typeof(Response<ServiceRequestResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> CreateRequest([FromBody] CreateServiceRequestDto request)
{
if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _createValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _requestService.CreateRequestAsync(request, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("user")]
[ProducesResponseType(typeof(Response<List<ServiceRequestResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetUserRequests()
{
var response = await _requestService.GetUserRequestsAsync(User);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("provider/{providerId}")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<List<ServiceRequestResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetProviderRequests(Guid providerId)
{
if (providerId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid provider ID"));

var response = await _requestService.GetProviderRequestsAsync(providerId);
return StatusCode((int)response.StatusCode, response);
}

[HttpPut("{requestId}/response")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<ServiceRequestResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> RespondToRequest(Guid requestId, [FromBody] ProviderResponseDto response)
{
if (requestId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

if (response == null)
return BadRequest(_responseHandler.BadRequest<object>("Response cannot be null"));

ValidationResult validationResult = await _responseValidator.ValidateAsync(response);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var result = await _requestService.RespondToRequestAsync(requestId, response, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("{requestId}/start")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<ServiceRequestResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> StartRequest(Guid requestId)
{
if (requestId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

var result = await _requestService.StartRequestAsync(requestId, User);
return StatusCode((int)result.StatusCode, result);
}

[HttpPost("{requestId}/complete")]
[Authorize(Roles = "Provider")]
[ProducesResponseType(typeof(Response<ServiceRequestResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> CompleteRequest(Guid requestId)
{
if (requestId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

var result = await _requestService.CompleteRequestAsync(requestId, User);
return StatusCode((int)result.StatusCode, result);
}
}

}
```

# ReviewController.cs
```cs
﻿using ElAnis.DataAccess.Services.Review;
using ElAnis.Entities.DTO.Review;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
private readonly IReviewService _reviewService;
private readonly ResponseHandler _responseHandler;
private readonly IValidator<CreateReviewDto> _createValidator;

public ReviewsController(
IReviewService reviewService,
ResponseHandler responseHandler,
IValidator<CreateReviewDto> createValidator)
{
_reviewService = reviewService;
_responseHandler = responseHandler;
_createValidator = createValidator;
}

[HttpPost]
[Authorize]
[ProducesResponseType(typeof(Response<ReviewResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto request)
{
if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _createValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _reviewService.CreateReviewAsync(request, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("provider/{providerId}")]
[ProducesResponseType(typeof(Response<ProviderReviewsResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetProviderReviews(Guid providerId)
{
if (providerId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid provider ID"));

var response = await _reviewService.GetProviderReviewsAsync(providerId);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("user")]
[Authorize]
[ProducesResponseType(typeof(Response<List<ReviewResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetUserReviews()
{
var response = await _reviewService.GetUserReviewsAsync(User);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("request/{requestId}")]
[Authorize]
[ProducesResponseType(typeof(Response<ReviewResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetReviewByRequestId(Guid requestId)
{
if (requestId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

var response = await _reviewService.GetReviewByRequestIdAsync(requestId);
return StatusCode((int)response.StatusCode, response);
}
}
}
```

# ServicePricingController.cs
```cs
﻿using ElAnis.DataAccess.Services.ServicePricing;
using ElAnis.Entities.DTO.ServicePricing;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class ServicePricingController : ControllerBase
{
private readonly IServicePricingService _servicePricingService;
private readonly ResponseHandler _responseHandler;
private readonly IValidator<CreateServicePricingRequest> _createValidator;
private readonly IValidator<UpdateServicePricingRequest> _updateValidator;
private readonly IValidator<BulkServicePricingRequest> _bulkValidator;

public ServicePricingController(
IServicePricingService servicePricingService,
ResponseHandler responseHandler,
IValidator<CreateServicePricingRequest> createValidator,
IValidator<UpdateServicePricingRequest> updateValidator,
IValidator<BulkServicePricingRequest> bulkValidator)
{
_servicePricingService = servicePricingService;
_responseHandler = responseHandler;
_createValidator = createValidator;
_updateValidator = updateValidator;
_bulkValidator = bulkValidator;
}

[HttpGet("categories-with-pricing")]
[ProducesResponseType(typeof(Response<List<CategoryWithPricingResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetAllCategoriesWithPricing()
{
var response = await _servicePricingService.GetAllCategoriesWithPricingAsync();
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("category/{categoryId}")]
[ProducesResponseType(typeof(Response<List<ServicePricingResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetByCategoryId(Guid categoryId)
{
if (categoryId == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

var response = await _servicePricingService.GetByCategoryIdAsync(categoryId);
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("active")]
[ProducesResponseType(typeof(Response<List<ServicePricingResponse>>), 200)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetActivePricing()
{
var response = await _servicePricingService.GetActivePricingAsync();
return StatusCode((int)response.StatusCode, response);
}

[HttpGet("{id}")]
[ProducesResponseType(typeof(Response<ServicePricingResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> GetById(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid pricing ID"));

var response = await _servicePricingService.GetByIdAsync(id);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<ServicePricingResponse>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> Create([FromBody] CreateServicePricingRequest request)
{
if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _createValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _servicePricingService.CreateAsync(request, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpPost("bulk")]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<List<ServicePricingResponse>>), 201)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> CreateBulk([FromBody] BulkServicePricingRequest request)
{
if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _bulkValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _servicePricingService.CreateBulkAsync(request, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpPut("{id}")]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<ServicePricingResponse>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServicePricingRequest request)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid pricing ID"));

if (request == null)
return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

ValidationResult validationResult = await _updateValidator.ValidateAsync(request);
if (!validationResult.IsValid)
{
string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
return BadRequest(_responseHandler.BadRequest<object>(errors));
}

var response = await _servicePricingService.UpdateAsync(id, request, User);
return StatusCode((int)response.StatusCode, response);
}

[HttpDelete("{id}")]
[Authorize(Policy = "AdminOnly")]
[ProducesResponseType(typeof(Response<string>), 200)]
[ProducesResponseType(typeof(Response<object>), 400)]
[ProducesResponseType(typeof(Response<object>), 401)]
[ProducesResponseType(typeof(Response<object>), 403)]
[ProducesResponseType(typeof(Response<object>), 404)]
[ProducesResponseType(typeof(Response<object>), 500)]
public async Task<IActionResult> Delete(Guid id)
{
if (id == Guid.Empty)
return BadRequest(_responseHandler.BadRequest<object>("Invalid pricing ID"));

var response = await _servicePricingService.DeleteAsync(id, User);
return StatusCode((int)response.StatusCode, response);
}
}
}
```

