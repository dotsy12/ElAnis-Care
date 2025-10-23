using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.Repositories
{
    public class ProviderAvailabilityRepository : GenericRepository<ProviderAvailability>, IProviderAvailabilityRepository
    {
        public ProviderAvailabilityRepository(AuthContext context) : base(context) { }

        public async Task<List<ProviderAvailability>> GetProviderAvailabilityAsync(
            Guid serviceProviderId,
            DateTime startDate,
            DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;

            return await _dbSet
                .Where(a => a.ServiceProviderId == serviceProviderId
                         && a.Date.Date >= start
                         && a.Date.Date <= end)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        public async Task<ProviderAvailability?> GetByDateAsync(Guid serviceProviderId, DateTime date)
        {
            var targetDate = date.Date;
            return await _dbSet
                .FirstOrDefaultAsync(a => a.ServiceProviderId == serviceProviderId
                                       && a.Date.Date == targetDate);
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
    }
}
