using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;

namespace ElAnis.DataAccess.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByServiceRequestIdAsync(Guid serviceRequestId);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
    }
}