using ElAnis.Entities.Models;
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