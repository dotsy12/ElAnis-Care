using ElAnis.Entities.DTO.Admin;

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
	}
}