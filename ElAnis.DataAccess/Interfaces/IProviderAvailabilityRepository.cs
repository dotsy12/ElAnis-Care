using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.Entities.Models;
using ElAnis.Utilities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.DataAccess.Interfaces
{
    public interface IProviderAvailabilityRepository : IGenericRepository<ProviderAvailability>
    {
        Task<List<ProviderAvailability>> GetProviderAvailabilityAsync(Guid serviceProviderId, DateTime startDate, DateTime endDate);
        Task<ProviderAvailability?> GetByDateAsync(Guid serviceProviderId, DateTime date);
        Task<bool> IsAvailableOnDateAsync(Guid serviceProviderId, DateTime date, ShiftType? shiftType = null);
        Task<List<DateTime>> GetBookedDatesAsync(Guid serviceProviderId, DateTime startDate, DateTime endDate);
    }
}
