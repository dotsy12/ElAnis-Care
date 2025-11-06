using ElAnis.Entities.DTO.ServiceRequest;
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
    }
}