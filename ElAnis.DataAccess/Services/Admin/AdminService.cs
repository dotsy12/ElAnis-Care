using ElAnis.DataAccess.ApplicationContext;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Admin
{
	public class AdminService : IAdminService
	{
		private readonly AuthContext _context;
		private readonly UserManager<User> _userManager;
		private readonly ResponseHandler _responseHandler;
		private readonly ILogger<AdminService> _logger;

		public AdminService(AuthContext context, UserManager<User> userManager, ResponseHandler responseHandler, ILogger<AdminService> logger)
		{
			_context = context;
			_userManager = userManager;
			_responseHandler = responseHandler;
			_logger = logger;
		}

		public async Task<Response<PaginatedResult<ServiceProviderApplicationDto>>> GetServiceProviderApplicationsAsync(int page, int pageSize)
		{
			try
			{
				var query = _context.ServiceProviderApplications
					.Include(a => a.User)
					.Include(a => a.ReviewedBy)
					.OrderByDescending(a => a.CreatedAt);

				var totalCount = await query.CountAsync();
				var applications = await query
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.Select(a => new ServiceProviderApplicationDto
					{
						Id = a.Id, // الآن Guid
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
					})
					.ToListAsync();

				var result = new PaginatedResult<ServiceProviderApplicationDto>
				{
					Items = applications,
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
				var application = await _context.ServiceProviderApplications
					.Include(a => a.User)
					.Include(a => a.ReviewedBy)
					.FirstOrDefaultAsync(a => a.Id == id); // مقارنة Guid بـ Guid مباشرة

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

		public async Task<Response<string>> ApproveServiceProviderApplicationAsync(Guid applicationId, ClaimsPrincipal adminClaims)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var adminId = adminClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(adminId))
					return _responseHandler.Unauthorized<string>("Admin not authenticated");

				var application = await _context.ServiceProviderApplications
					.Include(a => a.User)
					.FirstOrDefaultAsync(a => a.Id == applicationId); // مقارنة Guid بـ Guid مباشرة

				if (application == null)
					return _responseHandler.NotFound<string>("Application not found");

				if (application.Status != ServiceProviderApplicationStatus.Pending)
					return _responseHandler.BadRequest<string>("Application has already been reviewed");

				// Update application status
				application.Status = ServiceProviderApplicationStatus.Approved;
				application.ReviewedAt = DateTime.UtcNow;
				application.ReviewedById = adminId;

				// Change user role from USER to PROVIDER
				var user = application.User;
				await _userManager.RemoveFromRoleAsync(user, "USER");
				await _userManager.AddToRoleAsync(user, "PROVIDER");

				// Create ServiceProviderProfile
				var serviceProviderProfile = new ServiceProviderProfile
				{
					UserId = user.Id,
					Bio = application.Bio,
					NationalId = application.NationalId,
					Experience = application.Experience,
					HourlyRate = application.HourlyRate,
					IdDocumentPath = application.IdDocumentPath,
					CertificatePath = application.CertificatePath,
					Status = ServiceProviderStatus.Approved,
					IsAvailable = true,
					ApprovedAt = DateTime.UtcNow
				};

				_context.ServiceProviderProfiles.Add(serviceProviderProfile);

				// حفظ الـ ServiceProviderProfile الأول عشان نجيب الـ ID
				await _context.SaveChangesAsync();

				// Create categories relationships based on SelectedCategories
				if (!string.IsNullOrEmpty(application.SelectedCategories))
				{
					try
					{
						var categoryIds = System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(application.SelectedCategories);
						if (categoryIds?.Count > 0) // استخدم Count بدلاً من Any()
						{
							// تأكد إن الـ Categories موجودة
							var existingCategoryIds = await _context.Categories
								.Where(c => categoryIds.Contains(c.Id) && c.IsActive)
								.Select(c => c.Id)
								.ToListAsync();

							var categoryRelationships = existingCategoryIds.Select(catId => new ServiceProviderCategory
							{
								ServiceProviderId = serviceProviderProfile.Id,
								CategoryId = catId,
								CreatedAt = DateTime.UtcNow
							}).ToList();

							_context.ServiceProviderCategories.AddRange(categoryRelationships);
						}
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Failed to parse selected categories: {Categories}", application.SelectedCategories);
					}
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				_logger.LogInformation("Service provider application {ApplicationId} approved by admin {AdminId}", applicationId, adminId);
				return _responseHandler.Success<string>(null, "Application approved successfully");
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
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

				var application = await _context.ServiceProviderApplications
					.FirstOrDefaultAsync(a => a.Id == applicationId); // مقارنة Guid بـ Guid مباشرة

				if (application == null)
					return _responseHandler.NotFound<string>("Application not found");

				if (application.Status != ServiceProviderApplicationStatus.Pending)
					return _responseHandler.BadRequest<string>("Application has already been reviewed");

				application.Status = ServiceProviderApplicationStatus.Rejected;
				application.RejectionReason = rejectionReason;
				application.ReviewedAt = DateTime.UtcNow;
				application.ReviewedById = adminId;

				await _context.SaveChangesAsync();

				// TODO: Send rejection notification email

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

				// Basic counts
				stats.TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
				stats.TotalServiceProviders = await _context.ServiceProviderProfiles.CountAsync();
				stats.PendingApplications = await _context.ServiceProviderApplications.CountAsync(a => a.Status == ServiceProviderApplicationStatus.Pending);
				stats.TotalServiceRequests = await _context.ServiceRequests.CountAsync();
				stats.CompletedServiceRequests = await _context.ServiceRequests.CountAsync(sr => sr.Status == ServiceRequestStatus.Completed);
				stats.TotalReviews = await _context.Reviews.CountAsync();

				// Calculate total earnings (handle empty table)
				var serviceProviders = await _context.ServiceProviderProfiles.ToListAsync();
				stats.TotalEarnings = serviceProviders.Count > 0 ? serviceProviders.Sum(sp => sp.TotalEarnings) : 0; // استخدم Count بدلاً من Any()

				// Calculate average rating (handle division by zero)
				var providersWithReviews = serviceProviders.Where(sp => sp.TotalReviews > 0).ToList();
				stats.AverageRating = providersWithReviews.Count > 0 ? // استخدم Count بدلاً من Any()
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
				var query = _context.ServiceProviderProfiles
					.Include(sp => sp.User)
					.OrderByDescending(sp => sp.CreatedAt);

				var totalCount = await query.CountAsync();
				var serviceProviders = await query
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.Select(sp => new ServiceProviderDto
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
					})
					.ToListAsync();

				var result = new PaginatedResult<ServiceProviderDto>
				{
					Items = serviceProviders,
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
				var serviceProvider = await _context.ServiceProviderProfiles
					.FirstOrDefaultAsync(sp => sp.Id == serviceProviderId); // مقارنة Guid بـ Guid مباشرة

				if (serviceProvider == null)
					return _responseHandler.NotFound<string>("Service provider not found");

				serviceProvider.Status = ServiceProviderStatus.Suspended;
				serviceProvider.RejectionReason = reason;
				serviceProvider.IsAvailable = false;

				await _context.SaveChangesAsync();

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
				var serviceProvider = await _context.ServiceProviderProfiles
					.FirstOrDefaultAsync(sp => sp.Id == serviceProviderId); // مقارنة Guid بـ Guid مباشرة

				if (serviceProvider == null)
					return _responseHandler.NotFound<string>("Service provider not found");

				serviceProvider.Status = ServiceProviderStatus.Approved;
				serviceProvider.RejectionReason = null;
				serviceProvider.IsAvailable = true;

				await _context.SaveChangesAsync();

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
	}
}