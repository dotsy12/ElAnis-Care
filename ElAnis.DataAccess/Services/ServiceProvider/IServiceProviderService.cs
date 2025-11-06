using System.Security.Claims;
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

        // Application Status
        Task<Response<ApplicationStatusResponse>> GetApplicationStatusAsync(ClaimsPrincipal userClaims);
        // Dashboard
        Task<Response<ProviderDashboardResponse>> GetDashboardAsync(ClaimsPrincipal userClaims);

        // Profile
        Task<Response<ProviderProfileResponse>> GetProfileAsync(ClaimsPrincipal userClaims);
        Task<Response<ProviderProfileResponse>> UpdateProfileAsync(UpdateProviderProfileRequest request, ClaimsPrincipal userClaims);
        Task<Response<string>> ToggleAvailabilityAsync(ToggleAvailabilityRequest request, ClaimsPrincipal userClaims);

        // Working Areas
        Task<Response<List<Entities.DTO.WorkingArea.WorkingAreaDto>>> GetWorkingAreasAsync(ClaimsPrincipal userClaims);
        Task<Response<Entities.DTO.WorkingArea.WorkingAreaDto>> AddWorkingAreaAsync(AddWorkingAreaRequest request, ClaimsPrincipal userClaims);
        Task<Response<string>> DeleteWorkingAreaAsync(Guid workingAreaId, ClaimsPrincipal userClaims);
        // Availability
        Task<Response<AvailabilityCalendarResponse>> GetAvailabilityCalendarAsync(DateTime startDate, DateTime endDate, ClaimsPrincipal userClaims);
        Task<Response<Entities.DTO.Availability.AvailabilityDto>> AddAvailabilityAsync(AddAvailabilityRequest request, ClaimsPrincipal userClaims);
        Task<Response<Entities.DTO.Availability.AvailabilityDto>> UpdateAvailabilityAsync(UpdateAvailabilityRequest request, ClaimsPrincipal userClaims);
        Task<Response<string>> DeleteAvailabilityAsync(Guid availabilityId, ClaimsPrincipal userClaims);
        Task<Response<string>> AddBulkAvailabilityAsync(BulkAvailabilityRequest request, ClaimsPrincipal userClaims);
        Task<Response<PaginatedResult<ProviderSummaryResponse>>> SearchProvidersAsync(GetProvidersRequest request);
        Task<Response<ProviderDetailResponse>> GetProviderDetailAsync(Guid providerId);
    }
}
