using ElAnis.DataAccess.ApplicationContext;
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