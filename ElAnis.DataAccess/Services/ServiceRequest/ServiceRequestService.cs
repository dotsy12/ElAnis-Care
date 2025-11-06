using ElAnis.Entities.DTO.ServiceRequest;
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

                // Validate category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (category == null)
                    return _responseHandler.NotFound<ServiceRequestResponse>("Category not found");

                // Get pricing for server-side calculation
                var pricing = await _unitOfWork.ServicePricings.GetByShiftTypeAsync(request.CategoryId, request.ShiftType);
                if (pricing == null || !pricing.IsActive)
                    return _responseHandler.BadRequest<ServiceRequestResponse>("Pricing not available for selected category and shift");

                // If provider specified, validate availability
                if (request.ProviderId.HasValue)
                {
                    var provider = await _unitOfWork.ServiceProviderProfiles.GetByIdAsync(request.ProviderId.Value);
                    if (provider == null || provider.Status != ServiceProviderStatus.Approved)
                        return _responseHandler.NotFound<ServiceRequestResponse>("Provider not found or not approved");

                    var isAvailable = await _unitOfWork.ServiceProviderProfiles
                        .IsProviderAvailableOnDateAsync(request.ProviderId.Value, request.PreferredDate, request.ShiftType);

                    if (!isAvailable)
                        return _responseHandler.BadRequest<ServiceRequestResponse>("Provider is not available on the selected date and shift");

                    // Check for duplicate pending request
                    var hasPending = await _unitOfWork.ServiceRequests
                        .HasPendingRequestAsync(userId, request.ProviderId.Value, request.PreferredDate);

                    if (hasPending)
                        return _responseHandler.BadRequest<ServiceRequestResponse>("You already have a pending request with this provider for this date");
                }

                // Create request with server-calculated price
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
                // Verify provider exists
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

                // Verify this is the provider's request
                var providerProfile = await _unitOfWork.ServiceProviderProfiles
                    .FindSingleAsync(p => p.UserId == userId);

                if (providerProfile == null || serviceRequest.ServiceProviderId != providerProfile.Id)
                    return _responseHandler.Forbidden<ServiceRequestResponse>("You are not authorized to respond to this request");

                // Verify request is still pending
                if (serviceRequest.Status != ServiceRequestStatus.Pending)
                    return _responseHandler.BadRequest<ServiceRequestResponse>($"Request is already {serviceRequest.Status}");

                // Update request based on response
                if (response.Status == ServiceRequestStatus.Accepted)
                {
                    serviceRequest.Status = ServiceRequestStatus.Accepted;
                    serviceRequest.AcceptedAt = DateTime.UtcNow;

                    // 🔸 Notification part (disabled for now)
                    /*
                    var notification = new Notification
                    {
                        UserId = serviceRequest.UserId,
                        Title = "Request Approved",
                        Message = $"{providerProfile.User.FirstName} {providerProfile.User.LastName} approved your request for {serviceRequest.Category.Name}.",
                        Type = NotificationType.RequestAccepted,
                        ServiceRequestId = serviceRequest.Id,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Notifications.AddAsync(notification);
                    */
                }
                else if (response.Status == ServiceRequestStatus.Rejected)
                {
                    serviceRequest.Status = ServiceRequestStatus.Rejected;
                    serviceRequest.Description += $"\n[Rejection Reason: {response.Reason}]";

                    // 🔸 Notification part (disabled for now)
                    /*
                    var notification = new Notification
                    {
                        UserId = serviceRequest.UserId,
                        Title = "Request Rejected",
                        Message = $"{providerProfile.User.FirstName} {providerProfile.User.LastName} rejected your request. Reason: {response.Reason}",
                        Type = NotificationType.RequestRejected,
                        ServiceRequestId = serviceRequest.Id,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Notifications.AddAsync(notification);
                    */
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

        // Helper method to map entity to response DTO
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
                CanPay = request.Status == ServiceRequestStatus.Accepted && request.Payment == null
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
