using ElAnis.DataAccess.Services.Admin;
using ElAnis.Entities.DTO.Admin;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "AdminOnly")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _adminService;
		private readonly ResponseHandler _responseHandler;
		private readonly IValidator<RejectApplicationRequest> _rejectApplicationValidator;
		private readonly IValidator<SuspendServiceProviderRequest> _suspendServiceProviderValidator;

		public AdminController(
			IAdminService adminService,
			ResponseHandler responseHandler,
			IValidator<RejectApplicationRequest> rejectApplicationValidator,
			IValidator<SuspendServiceProviderRequest> suspendServiceProviderValidator)
		{
			_adminService = adminService;
			_responseHandler = responseHandler;
			_rejectApplicationValidator = rejectApplicationValidator;
			_suspendServiceProviderValidator = suspendServiceProviderValidator;
		}

		[HttpGet("dashboard-stats")]
		public async Task<IActionResult> GetDashboardStats()
		{
			var response = await _adminService.GetDashboardStatsAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpGet("service-provider-applications")]
		public async Task<IActionResult> GetServiceProviderApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			if (page < 1) page = 1;
			if (pageSize < 1 || pageSize > 100) pageSize = 10;

			var response = await _adminService.GetServiceProviderApplicationsAsync(page, pageSize);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpGet("service-provider-applications/{id}")]
		public async Task<IActionResult> GetServiceProviderApplicationById(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

			var response = await _adminService.GetServiceProviderApplicationByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpPost("service-provider-applications/{id}/approve")]
		public async Task<IActionResult> ApproveServiceProviderApplication(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid application ID"));

			var response = await _adminService.ApproveServiceProviderApplicationAsync(id, User);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpPost("service-provider-applications/{id}/reject")]
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

		[HttpPost("service-providers/{id}/suspend")]
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

		[HttpPost("service-providers/{id}/activate")]
		public async Task<IActionResult> ActivateServiceProvider(Guid id)
		{
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid service provider ID"));

			var response = await _adminService.ActivateServiceProviderAsync(id, User);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}