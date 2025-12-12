using ElAnis.DataAccess;
using ElAnis.DataAccess.Services.Admin;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Controller for administrative operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUnitOfWork _unitOfWork; // ✅ أضف هذا
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<RejectApplicationRequest> _rejectApplicationValidator;
        private readonly IValidator<SuspendServiceProviderRequest> _suspendServiceProviderValidator;

        /// <summary>
        /// Initializes a new instance of AdminController
        /// </summary>
        public AdminController(
            IAdminService adminService,
            IUnitOfWork unitOfWork, // ✅ أضف هذا
            ResponseHandler responseHandler,
            IValidator<RejectApplicationRequest> rejectApplicationValidator,
            IValidator<SuspendServiceProviderRequest> suspendServiceProviderValidator)
        {
            _adminService = adminService;
            _unitOfWork = unitOfWork; // ✅ أضف هذا
            _responseHandler = responseHandler;
            _rejectApplicationValidator = rejectApplicationValidator;
            _suspendServiceProviderValidator = suspendServiceProviderValidator;
        }


        /// <summary>
        /// Retrieves dashboard statistics for admin panel
        /// </summary>
        /// <returns>Dashboard statistics including user counts, applications, and system metrics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin access required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("dashboard-stats")]
		[ProducesResponseType(typeof(Response<AdminDashboardStatsDto>), 200)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> GetDashboardStats()
		{
			var response = await _adminService.GetDashboardStatsAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Retrieves paginated list of service provider applications
		/// </summary>
		/// <param name="page">Page number (default: 1, minimum: 1)</param>
		/// <param name="pageSize">Number of items per page (default: 10, minimum: 1, maximum: 100)</param>
		/// <returns>Paginated list of service provider applications</returns>
		/// <response code="200">Applications retrieved successfully</response>
		/// <response code="400">Invalid pagination parameters</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("service-provider-applications")]
		[ProducesResponseType(typeof(Response<PaginatedResult<ServiceProviderApplicationDto>>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> GetServiceProviderApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			if (page < 1) page = 1;
			if (pageSize < 1 || pageSize > 100) pageSize = 10;

			var response = await _adminService.GetServiceProviderApplicationsAsync(page, pageSize);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Retrieves detailed information about a specific service provider application
		/// </summary>
		/// <param name="id">The unique identifier of the application</param>
		/// <returns>Detailed application information including documents and personal data</returns>
		/// <response code="200">Application details retrieved successfully</response>
		/// <response code="400">Invalid application ID provided</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="404">Application not found</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("service-provider-applications/{id}")]
		[ProducesResponseType(typeof(Response<ServiceProviderApplicationDetailDto>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
      
        public async Task<IActionResult> GetServiceProviderApplicationById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

            var application = await _unitOfWork.ServiceProviderApplications
                .GetApplicationWithDetailsAsync(id);

            if (application == null)
                return NotFound(_responseHandler.NotFound<object>("Application not found"));

            // ✅ Return with document URLs
            var response = new
            {
                application.Id,
                application.FirstName,
                application.LastName,
                
                application.PhoneNumber,
                application.Bio,
                application.Experience,
                application.HourlyRate,
                Documents = new
                {
                    IdDocument = application.IdDocumentPath,
                    Certificate = application.CertificatePath,
                    CV = application.CVPath
                },
                application.Status,
                application.CreatedAt,
                application.ReviewedAt
            };

            return Ok(_responseHandler.Success(response, "Application retrieved successfully"));
        }

        /// <summary>
        /// Approves a service provider application and creates service provider profile
        /// </summary>
        /// <param name="id">The unique identifier of the application to approve</param>
        /// <returns>Approval confirmation</returns>
        /// <remarks>
        /// This action will:
        /// - Change application status to Approved
        /// - Change user role from USER to PROVIDER  
        /// - Create ServiceProviderProfile
        /// - Link selected categories to the service provider
        /// </remarks>
        /// <response code="200">Application approved successfully</response>
        /// <response code="400">Invalid application ID or application already reviewed</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin access required</response>
        /// <response code="404">Application not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("service-provider-applications/{id}/approve")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> ApproveServiceProviderApplication(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

			var response = await _adminService.ApproveServiceProviderApplicationAsync(id, User);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Rejects a service provider application with a specified reason
		/// </summary>
		/// <param name="id">The unique identifier of the application to reject</param>
		/// <param name="request">The rejection request containing the reason for rejection</param>
		/// <returns>Rejection confirmation</returns>
		/// <response code="200">Application rejected successfully</response>
		/// <response code="400">Invalid application ID, request data, or validation errors</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="404">Application not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("service-provider-applications/{id}/reject")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> RejectServiceProviderApplication(Guid id, [FromBody] RejectApplicationRequest request)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

			ValidationResult validationResult = await _rejectApplicationValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BadRequest(_responseHandler.BadRequest<object>(errors));
			}

			var response = await _adminService.RejectServiceProviderApplicationAsync(id, request.RejectionReason, User);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Suspends an active service provider with a specified reason
		/// </summary>
		/// <param name="id">The unique identifier of the service provider to suspend</param>
		/// <param name="request">The suspension request containing the reason for suspension</param>
		/// <returns>Suspension confirmation</returns>
		/// <response code="200">Service provider suspended successfully</response>
		/// <response code="400">Invalid service provider ID, request data, or validation errors</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="404">Service provider not found</response>
		/// <response code="409">Conflict - Service provider already suspended or inactive</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("service-providers/{id}/suspend")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 409)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> SuspendServiceProvider(Guid id, [FromBody] SuspendServiceProviderRequest request)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid service provider ID"));

			ValidationResult validationResult = await _suspendServiceProviderValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BadRequest(_responseHandler.BadRequest<object>(errors));
			}

			var response = await _adminService.SuspendServiceProviderAsync(id, request.Reason, User);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Activates a suspended service provider
		/// </summary>
		/// <param name="id">The unique identifier of the service provider to activate</param>
		/// <returns>Activation confirmation</returns>
		/// <response code="200">Service provider activated successfully</response>
		/// <response code="400">Invalid service provider ID provided</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="404">Service provider not found</response>
		/// <response code="409">Conflict - Service provider already active</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("service-providers/{id}/activate")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 409)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> ActivateServiceProvider(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid service provider ID"));

			var response = await _adminService.ActivateServiceProviderAsync(id, User);
			return StatusCode((int)response.StatusCode, response);
		}


        /// <summary>
        /// Get all users with filtering and pagination
        /// </summary>
        [HttpGet("users")]
        [ProducesResponseType(typeof(Response<PaginatedResult<UserManagementDto>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
        {
            if (request.Page < 1) request.Page = 1;
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

            var response = await _adminService.GetUsersAsync(request);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Suspend a user account
        /// </summary>
        [HttpPost("users/{userId}/suspend")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> SuspendUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(_responseHandler.BadRequest<object>("Invalid user ID"));

            var response = await _adminService.SuspendUserAsync(userId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Activate a suspended user account
        /// </summary>
        [HttpPost("users/{userId}/activate")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(_responseHandler.BadRequest<object>("Invalid user ID"));

            var response = await _adminService.ActivateUserAsync(userId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get recent bookings
        /// </summary>
        [HttpGet("bookings/recent")]
        [ProducesResponseType(typeof(Response<List<RecentBookingDto>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetRecentBookings([FromQuery] int limit = 10)
        {
            if (limit < 1 || limit > 100) limit = 10;

            var response = await _adminService.GetRecentBookingsAsync(limit);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all payment transactions with total revenue
        /// </summary>
        [HttpGet("payments")]
        [ProducesResponseType(typeof(Response<PaymentSummaryResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetPaymentTransactions()
        {
            var response = await _adminService.GetPaymentTransactionsAsync();
            return StatusCode((int)response.StatusCode, response);
        }


    }
}