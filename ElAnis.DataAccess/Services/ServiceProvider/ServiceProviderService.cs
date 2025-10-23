using ElAnis.DataAccess;
using ElAnis.DataAccess.Services.ServiceProvider;
using ElAnis.Entities.DTO.Availability;
using ElAnis.Entities.DTO.ServiceProviderProfile;
using ElAnis.Entities.DTO.WorkingArea;
using ElAnis.Entities.Models;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class ServiceProviderService : IServiceProviderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ServiceProvider> _logger;
    private readonly ResponseHandler _responseHandler;
    // private readonly IFileUploadService _fileUploadService; // إذا كان موجود

    public ServiceProviderService(
        IUnitOfWork unitOfWork,
        ILogger<ServiceProvider> logger,
        ResponseHandler responseHandler)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _responseHandler = responseHandler;
    }

    // ===== APPLICATION STATUS =====
    public async Task<Response<ApplicationStatusResponse>> GetApplicationStatusAsync(ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<ApplicationStatusResponse>("User not authenticated");

            var application = await _unitOfWork.ServiceProviderApplications
                .FindSingleAsync(a => a.UserId == userId);

            if (application == null)
                return _responseHandler.NotFound<ApplicationStatusResponse>("No application found");

            // ✅ تحقق من وجود Profile
            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            // ✅ تحقق من الـ Token Claim
            var statusClaim = userClaims.FindFirst("ServiceProviderStatus")?.Value;
            bool needsTokenRefresh = false;
            string? nextAction = null;
            string? refreshEndpoint = null;

            if (application.Status == ServiceProviderApplicationStatus.Approved)
            {
                // إذا كان Approved لكن مفيش Profile
                if (profile == null)
                {
                    nextAction = "Your application was approved but profile creation failed. Please contact support.";
                }
                // إذا كان Approved وفي Profile لكن الـ Token مش محدث
                else if (statusClaim != "Approved")
                {
                    needsTokenRefresh = true;
                    nextAction = "Please refresh your access token to access the provider dashboard.";
                    refreshEndpoint = "/api/auth/refresh-token";
                }
                else
                {
                    nextAction = "You can now access your provider dashboard.";
                }
            }

            var response = new ApplicationStatusResponse
            {
                ApplicationId = application.Id,
                Status = application.Status,
                StatusText = GetStatusText(application.Status),
                Message = GetStatusMessage(application.Status),
                RejectionReason = application.RejectionReason,
                CreatedAt = application.CreatedAt,
                ReviewedAt = application.ReviewedAt,
                CanReapply = application.Status == ServiceProviderApplicationStatus.Rejected,
                HasProfile = profile != null,
                NeedsTokenRefresh = needsTokenRefresh,
                NextAction = nextAction,
                RefreshTokenEndpoint = refreshEndpoint
            };

            return _responseHandler.Success(response, "Application status retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application status");
            return _responseHandler.ServerError<ApplicationStatusResponse>("Error retrieving application status");
        }
    }

 
    // ===== DASHBOARD =====
    public async Task<Response<ProviderDashboardResponse>> GetDashboardAsync(ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<ProviderDashboardResponse>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .GetQueryable()
                .Include(p => p.User)
                .Include(p => p.Categories).ThenInclude(c => c.Category)
                .Include(p => p.WorkingAreas)
                .Include(p => p.ServiceRequests)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                // ✅ لو مفيش Profile معناها لسه مش متوافق عليه
                return _responseHandler.Forbidden<ProviderDashboardResponse>(
                    "Your application is still pending or has been rejected. " +
                    "Please check your application status at /api/Provider/application-status");
            }

            // ✅ تحقق من إن الـ Profile متوافق عليه
            if (profile.Status != ServiceProviderStatus.Approved)
            {
                return _responseHandler.Forbidden<ProviderDashboardResponse>(
                    $"Your provider status is {profile.Status}. Only approved providers can access the dashboard. " +
                    "If you were recently approved, please log out and log in again.");
            }

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Get statistics
            var pendingRequests = await _unitOfWork.Repository<ServiceRequest>()
                .CountAsync(r => r.ServiceProviderId == profile.Id && r.Status == ServiceRequestStatus.Pending);

            var upcomingJobs = await _unitOfWork.Repository<ServiceRequest>()
                .CountAsync(r => r.ServiceProviderId == profile.Id
                              && (r.Status == ServiceRequestStatus.Accepted || r.Status == ServiceRequestStatus.Paid)
                              && r.PreferredDate >= now);

            var currentMonthEarnings = await _unitOfWork.Repository<ServiceRequest>()
                .GetQueryable()
                .Where(r => r.ServiceProviderId == profile.Id
                         && r.Status == ServiceRequestStatus.Completed
                         && r.CompletedAt >= startOfMonth)
                .SumAsync(r => r.TotalPrice);

            // Recent Requests (last 5)
            var recentRequests = await _unitOfWork.Repository<ServiceRequest>()
                .GetQueryable()
                .Where(r => r.ServiceProviderId == profile.Id)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Include(r => r.User)
                .Include(r => r.Category)
                .Select(r => new ServiceRequestSummary
                {
                    Id = r.Id,
                    ClientName = r.User.FirstName + " " + r.User.LastName,
                    CategoryName = r.Category.Name,
                    PreferredDate = r.PreferredDate,
                    ShiftType = r.ShiftType,
                    ShiftTypeName = r.ShiftType.ToString(),
                    Status = r.Status,
                    StatusText = r.Status.ToString(),
                    Price = r.TotalPrice,
                    Address = r.Address,
                    Governorate = r.Governorate
                })
                .ToListAsync();

            // Upcoming Jobs (next 5)
            var upcomingJobsList = await _unitOfWork.Repository<ServiceRequest>()
                .GetQueryable()
                .Where(r => r.ServiceProviderId == profile.Id
                         && (r.Status == ServiceRequestStatus.Accepted || r.Status == ServiceRequestStatus.Paid)
                         && r.PreferredDate >= now)
                .OrderBy(r => r.PreferredDate)
                .Take(5)
                .Include(r => r.User)
                .Include(r => r.Category)
                .Select(r => new ServiceRequestSummary
                {
                    Id = r.Id,
                    ClientName = r.User.FirstName + " " + r.User.LastName,
                    CategoryName = r.Category.Name,
                    PreferredDate = r.PreferredDate,
                    ShiftType = r.ShiftType,
                    ShiftTypeName = r.ShiftType.ToString(),
                    Status = r.Status,
                    StatusText = r.Status.ToString(),
                    Price = r.TotalPrice,
                    Address = r.Address,
                    Governorate = r.Governorate
                })
                .ToListAsync();

            var response = new ProviderDashboardResponse
            {
                ProfileId = profile.Id,
                FullName = profile.User.FirstName + " " + profile.User.LastName,
                Email = profile.User.Email ?? string.Empty,
                ProfilePicture = profile.User.ProfilePicture,
                IsAvailable = profile.IsAvailable,
                Status = profile.Status,
                Statistics = new DashboardStatistics
                {
                    CompletedJobs = profile.CompletedJobs,
                    PendingRequests = pendingRequests,
                    UpcomingJobs = upcomingJobs,
                    TotalEarnings = profile.TotalEarnings,
                    CurrentMonthEarnings = currentMonthEarnings,
                    AverageRating = profile.AverageRating,
                    TotalReviews = profile.TotalReviews,
                    WorkedDays = profile.WorkedDays
                },
                RecentRequests = recentRequests,
                UpcomingJobs = upcomingJobsList,
                Categories = profile.Categories.Select(c => new CategorySummary
                {
                    Id = c.CategoryId,
                    Name = c.Category.Name,
                    Icon = c.Category.Icon
                }).ToList(),
                WorkingAreas = profile.WorkingAreas
                    .Where(w => w.IsActive)
                    .Select(w => w.Governorate)
                    .Distinct()
                    .ToList()
            };

            return _responseHandler.Success(response, "Dashboard retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider dashboard");
            return _responseHandler.ServerError<ProviderDashboardResponse>("Error retrieving dashboard");
        }
    }
    // ===== PROFILE =====
    public async Task<Response<ProviderProfileResponse>> GetProfileAsync(ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<ProviderProfileResponse>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .GetQueryable()
                .Include(p => p.User)
                .Include(p => p.Categories).ThenInclude(c => c.Category)
                .Include(p => p.WorkingAreas)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<ProviderProfileResponse>("Profile not found");

            var response = new ProviderProfileResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FirstName = profile.User.FirstName,
                LastName = profile.User.LastName,
                Email = profile.User.Email ?? string.Empty,
                PhoneNumber = profile.User.PhoneNumber ?? string.Empty,
                ProfilePicture = profile.User.ProfilePicture,
                Bio = profile.Bio,
                Experience = profile.Experience,
                NationalId = profile.NationalId,
                IsAvailable = profile.IsAvailable,
                Status = profile.Status,
                CompletedJobs = profile.CompletedJobs,
                TotalEarnings = profile.TotalEarnings,
                AverageRating = profile.AverageRating,
                TotalReviews = profile.TotalReviews,
                Categories = profile.Categories.Select(c => new CategorySummary
                {
                    Id = c.CategoryId,
                    Name = c.Category.Name,
                    Icon = c.Category.Icon
                }).ToList(),
                WorkingAreas = profile.WorkingAreas
                    .Where(w => w.IsActive)
                    .Select(w => new WorkingAreaDto
                    {
                        Id = w.Id,
                        Governorate = w.Governorate,
                        City = w.City,
                        District = w.District,
                        IsActive = w.IsActive
                    }).ToList()
            };

            return _responseHandler.Success(response , "Profile retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting provider profile");
            return _responseHandler.ServerError<ProviderProfileResponse>("Error retrieving profile");
        }
    }


    public async Task<Response<ProviderProfileResponse>> UpdateProfileAsync(
           UpdateProviderProfileRequest request,
           ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<ProviderProfileResponse>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<ProviderProfileResponse>("Profile not found");

            // Update fields
            if (!string.IsNullOrEmpty(request.Bio))
                profile.Bio = request.Bio;

            if (!string.IsNullOrEmpty(request.Experience))
                profile.Experience = request.Experience;

            // Handle Profile Picture Upload
            if (request.ProfilePicture != null)
            {
                // var profilePicturePath = await _fileUploadService.UploadFileAsync(request.ProfilePicture, "profile-pictures");
                // profile.User.ProfilePicture = profilePicturePath;
            }

            _unitOfWork.ServiceProviderProfiles.Update(profile);
            await _unitOfWork.CompleteAsync();

            return await GetProfileAsync(userClaims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating provider profile");
            return _responseHandler.ServerError<ProviderProfileResponse>("Error updating profile");
        }
    }

    public async Task<Response<string>> ToggleAvailabilityAsync(
        ToggleAvailabilityRequest request,
        ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<string>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<string>("Profile not found");

            profile.IsAvailable = request.IsAvailable;
            _unitOfWork.ServiceProviderProfiles.Update(profile);
            await _unitOfWork.CompleteAsync();

            var message = request.IsAvailable ? "You are now available for work" : "You are now unavailable";
            return _responseHandler.Success<string>(null, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling availability");
            return _responseHandler.ServerError<string>("Error updating availability");
        }
    }

    // ===== WORKING AREAS =====
    public async Task<Response<List<WorkingAreaDto>>> GetWorkingAreasAsync(ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<List<WorkingAreaDto>>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<List<WorkingAreaDto>>("Profile not found");

            var workingAreas = await _unitOfWork.ProviderWorkingAreas
                .GetProviderWorkingAreasAsync(profile.Id);

            var response = workingAreas.Select(w => new WorkingAreaDto
            {
                Id = w.Id,
                Governorate = w.Governorate,
                City = w.City,
                District = w.District,
                IsActive = w.IsActive
            }).ToList();

            return _responseHandler.Success(response, "Working Area retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting working areas");
            return _responseHandler.ServerError<List<WorkingAreaDto>>("Error retrieving working areas");
        }
    }

    public async Task<Response<WorkingAreaDto>> AddWorkingAreaAsync(
        AddWorkingAreaRequest request,
        ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<WorkingAreaDto>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<WorkingAreaDto>("Profile not found");

            // Check if governorate already exists
            var exists = await _unitOfWork.ProviderWorkingAreas
                .IsGovernorateExistsAsync(profile.Id, request.Governorate);

            if (exists)
                return _responseHandler.BadRequest<WorkingAreaDto>("This governorate already exists in your working areas");

            var workingArea = new ProviderWorkingArea
            {
                ServiceProviderId = profile.Id,
                Governorate = request.Governorate,
                City = request.City,
                District = request.District,
                IsActive = true
            };

            await _unitOfWork.ProviderWorkingAreas.AddAsync(workingArea);
            await _unitOfWork.CompleteAsync();

            var response = new WorkingAreaDto
            {
                Id = workingArea.Id,
                Governorate = workingArea.Governorate,
                City = workingArea.City,
                District = workingArea.District,
                IsActive = workingArea.IsActive
            };

            return _responseHandler.Created(response, "Working area added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding working area");
            return _responseHandler.ServerError<WorkingAreaDto>("Error adding working area");
        }
    }

    public async Task<Response<string>> DeleteWorkingAreaAsync(Guid workingAreaId, ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<string>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<string>("Profile not found");

            var workingArea = await _unitOfWork.ProviderWorkingAreas
                .FindSingleAsync(w => w.Id == workingAreaId && w.ServiceProviderId == profile.Id);

            if (workingArea == null)
                return _responseHandler.NotFound<string>("Working area not found");

            // Soft delete
            workingArea.IsActive = false;
            _unitOfWork.ProviderWorkingAreas.Update(workingArea);
            await _unitOfWork.CompleteAsync();

            return _responseHandler.Success<string>(null, "Working area deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting working area");
            return _responseHandler.ServerError<string>("Error deleting working area");
        }
    }

    // ===== AVAILABILITY =====
    public async Task<Response<AvailabilityCalendarResponse>> GetAvailabilityCalendarAsync(
        DateTime startDate,
        DateTime endDate,
        ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<AvailabilityCalendarResponse>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<AvailabilityCalendarResponse>("Profile not found");

            // Get availability
            var availability = await _unitOfWork.ProviderAvailabilities
                .GetProviderAvailabilityAsync(profile.Id, startDate, endDate);

            // Get booked dates
            var bookedRequests = await _unitOfWork.Repository<ServiceRequest>()
                .GetQueryable()
                .Where(r => r.ServiceProviderId == profile.Id
                         && r.PreferredDate.Date >= startDate.Date
                         && r.PreferredDate.Date <= endDate.Date
                         && (r.Status == ServiceRequestStatus.Accepted
                             || r.Status == ServiceRequestStatus.Paid
                             || r.Status == ServiceRequestStatus.InProgress))
                .Include(r => r.User)
                .Include(r => r.Category)
                .Select(r => new ServiceRequestSummary
                {
                    Id = r.Id,
                    ClientName = r.User.FirstName + " " + r.User.LastName,
                    CategoryName = r.Category.Name,
                    PreferredDate = r.PreferredDate,
                    ShiftType = r.ShiftType,
                    ShiftTypeName = r.ShiftType.ToString(),
                    Status = r.Status,
                    StatusText = r.Status.ToString(),
                    Price = r.TotalPrice,
                    Address = r.Address,
                    Governorate = r.Governorate
                })
                .ToListAsync();

            var response = new AvailabilityCalendarResponse
            {
                Availability = availability.Select(a => new AvailabilityDto
                {
                    Id = a.Id,
                    Date = a.Date,
                    IsAvailable = a.IsAvailable,
                    AvailableShift = a.AvailableShift,
                    Notes = a.Notes
                }).ToList(),
                BookedDates = bookedRequests
            };

            return _responseHandler.Success(response, "Availability  retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting availability calendar");
            return _responseHandler.ServerError<AvailabilityCalendarResponse>("Error retrieving calendar");
        }
    }


    public async Task<Response<AvailabilityDto>> AddAvailabilityAsync(
            AddAvailabilityRequest request,
            ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<AvailabilityDto>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<AvailabilityDto>("Profile not found");

            // Check if date already exists
            var existing = await _unitOfWork.ProviderAvailabilities
                .GetByDateAsync(profile.Id, request.Date);

            if (existing != null)
                return _responseHandler.BadRequest<AvailabilityDto>("Availability for this date already exists. Please update it instead.");

            var availability = new ProviderAvailability
            {
                ServiceProviderId = profile.Id,
                Date = request.Date.Date,
                IsAvailable = request.IsAvailable,
                AvailableShift = request.AvailableShift,
                Notes = request.Notes
            };

            await _unitOfWork.ProviderAvailabilities.AddAsync(availability);
            await _unitOfWork.CompleteAsync();

            var response = new AvailabilityDto
            {
                Id = availability.Id,
                Date = availability.Date,
                IsAvailable = availability.IsAvailable,
                AvailableShift = availability.AvailableShift,
                Notes = availability.Notes
            };

            return _responseHandler.Created(response, "Availability added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding availability");
            return _responseHandler.ServerError<AvailabilityDto>("Error adding availability");
        }
    }


    public async Task<Response<AvailabilityDto>> UpdateAvailabilityAsync(
           UpdateAvailabilityRequest request,
           ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<AvailabilityDto>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<AvailabilityDto>("Profile not found");

            var availability = await _unitOfWork.ProviderAvailabilities
                .FindSingleAsync(a => a.Id == request.Id && a.ServiceProviderId == profile.Id);

            if (availability == null)
                return _responseHandler.NotFound<AvailabilityDto>("Availability not found");

            availability.IsAvailable = request.IsAvailable;
            availability.AvailableShift = request.AvailableShift;
            availability.Notes = request.Notes;
            availability.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ProviderAvailabilities.Update(availability);
            await _unitOfWork.CompleteAsync();

            var response = new AvailabilityDto
            {
                Id = availability.Id,
                Date = availability.Date,
                IsAvailable = availability.IsAvailable,
                AvailableShift = availability.AvailableShift,
                Notes = availability.Notes
            };

            return _responseHandler.Success(response, "Availability updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating availability");
            return _responseHandler.ServerError<AvailabilityDto>("Error updating availability");
        }
    }




    public async Task<Response<string>> DeleteAvailabilityAsync(Guid availabilityId, ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<string>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<string>("Profile not found");

            var availability = await _unitOfWork.ProviderAvailabilities
                .FindSingleAsync(a => a.Id == availabilityId && a.ServiceProviderId == profile.Id);

            if (availability == null)
                return _responseHandler.NotFound<string>("Availability not found");

            _unitOfWork.ProviderAvailabilities.Delete(availability);
            await _unitOfWork.CompleteAsync();

            return _responseHandler.Success<string>(null, "Availability deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting availability");
            return _responseHandler.ServerError<string>("Error deleting availability");
        }
    }

    public async Task<Response<string>> AddBulkAvailabilityAsync(
        BulkAvailabilityRequest request,
        ClaimsPrincipal userClaims)
    {
        try
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return _responseHandler.Unauthorized<string>("User not authenticated");

            var profile = await _unitOfWork.ServiceProviderProfiles
                .FindSingleAsync(p => p.UserId == userId);

            if (profile == null)
                return _responseHandler.NotFound<string>("Profile not found");

            var availabilityList = new List<ProviderAvailability>();
            var currentDate = request.StartDate.Date;

            while (currentDate <= request.EndDate.Date)
            {
                // Skip excluded days
                if (request.ExcludeDays != null && request.ExcludeDays.Contains(currentDate.DayOfWeek))
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                // Check if already exists
                var existing = await _unitOfWork.ProviderAvailabilities
                    .GetByDateAsync(profile.Id, currentDate);

                if (existing == null)
                {
                    availabilityList.Add(new ProviderAvailability
                    {
                        ServiceProviderId = profile.Id,
                        Date = currentDate,
                        IsAvailable = request.IsAvailable,
                        AvailableShift = request.AvailableShift
                    });
                }

                currentDate = currentDate.AddDays(1);
            }

            if (availabilityList.Any())
            {
                await _unitOfWork.ProviderAvailabilities.AddRangeAsync(availabilityList);
                await _unitOfWork.CompleteAsync();
            }

            return _responseHandler.Success<string>(null, $"{availabilityList.Count} availability records added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bulk availability");
            return _responseHandler.ServerError<string>("Error adding bulk availability");
        }
    }
    private string GetStatusText(ServiceProviderApplicationStatus status)
    {
        return status switch
        {
            ServiceProviderApplicationStatus.Pending => "Pending Review",
            ServiceProviderApplicationStatus.Approved => "Approved",
            ServiceProviderApplicationStatus.Rejected => "Rejected",
            _ => "Unknown"
        };
    }
    private string GetStatusMessage(ServiceProviderApplicationStatus status)
    {
        return status switch
        {
            ServiceProviderApplicationStatus.Pending => "Your application is currently under review.",
            ServiceProviderApplicationStatus.Approved => "Your application has been approved. You can now access your provider dashboard.",
            ServiceProviderApplicationStatus.Rejected => "Your application was rejected. You can check the rejection reason and reapply.",
            _ => "Unknown status."
        };
    }


}