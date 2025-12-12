using System.Security.Claims;
using ElAnis.DataAccess.Repositories;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.AspNetCore.Identity;
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

                // ✅ 1. Update application status
                application.Status = ServiceProviderApplicationStatus.Approved;
                application.ReviewedAt = DateTime.UtcNow;
                application.ReviewedById = adminId;
                _unitOfWork.ServiceProviderApplications.Update(application);

                var user = application.User;

                // ✅ 2. Create ServiceProviderProfile
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

                // ✅ 3. Handle categories
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

                // Use repositories for counts
                stats.TotalUsers = await _unitOfWork.Users.CountAsync(u => !u.IsDeleted);
                stats.TotalServiceProviders = await _unitOfWork.ServiceProviderProfiles.CountAsync();
                stats.PendingApplications = await _unitOfWork.ServiceProviderApplications.CountAsync(
                    a => a.Status == ServiceProviderApplicationStatus.Pending);

                // Get additional stats using generic repository
                var serviceRequestRepo = _unitOfWork.Repository<ElAnis.Entities.Models.ServiceRequest>();
                var reviewRepo = _unitOfWork.Repository<ElAnis.Entities.Models.Review>();


                stats.TotalServiceRequests = await serviceRequestRepo.CountAsync();
                stats.CompletedServiceRequests = await serviceRequestRepo.CountAsync(
                    sr => sr.Status == ServiceRequestStatus.Completed);
                stats.TotalReviews = await reviewRepo.CountAsync();

                // Calculate earnings and ratings
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


        // ✅ 1. User Management
        public async Task<Response<PaginatedResult<UserManagementDto>>> GetUsersAsync(GetUsersRequest request)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // Search
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var searchLower = request.Search.ToLower();
                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(searchLower) ||
                        u.LastName.ToLower().Contains(searchLower) ||
                        u.Email.ToLower().Contains(searchLower) ||
                        u.PhoneNumber.Contains(searchLower));
                }

                // Filter by Status
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

                // Filter by Role if specified
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

        // ✅ 2. Suspend User
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

        // ✅ 3. Activate User
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

        // ✅ 4. Recent Bookings
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

        // ✅ 5. Payment Transactions
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

        // ✅ Helper Methods
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