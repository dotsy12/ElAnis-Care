using ElAnis.DataAccess.Services.ServiceProvider;
using ElAnis.Entities.DTO.Availability;
using ElAnis.Entities.DTO.ServiceProviderProfile;
using ElAnis.Entities.DTO.WorkingArea;
using ElAnis.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Controller for service provider operations (status, dashboard, profile, availability, and working areas).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IServiceProviderService _providerService;
        private readonly ResponseHandler _responseHandler;

        public ProviderController(IServiceProviderService providerService, ResponseHandler responseHandler)
        {
            _providerService = providerService;
            _responseHandler = responseHandler;
        }

        // ===== APPLICATION STATUS =====
        /// <summary>
        /// Retrieves the application status of the current user (USER or PROVIDER).
        /// يمكن لأي مستخدم معمول login يشوف حالة طلبه
        /// </summary>
        /// <returns>Application status details.</returns>
        /// <response code="200">Application status retrieved successfully.</response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="404">Application not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("application-status")]
        [Authorize] // ✅ أي حد معمول login يقدر يشوف حالة طلبه
        [ProducesResponseType(typeof(Response<ApplicationStatusResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetApplicationStatus()
        {
            var result = await _providerService.GetApplicationStatusAsync(User);
            return StatusCode((int)result.StatusCode, result);
        }

        // ===== DASHBOARD =====
        /// <summary>
        /// Retrieves the dashboard data for the approved provider (للصفحة: /provider/dashboard).
        /// ⚠️ مسموح فقط للبروفايدرز المقبولين (Approved)
        /// 💡 لو طلع 403 Forbidden → معناها المستخدم لازم يعمل Login مرة تانية بعد الموافقة
        /// </summary>
        /// <returns>Provider dashboard data.</returns>
        /// <response code="200">Dashboard data retrieved successfully.</response>
        /// <response code="401">Unauthorized - Authentication required.</response>
        /// <response code="403">Forbidden - Provider not approved OR needs to re-login.</response>
        /// <response code="404">Profile not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Provider")] // ✅ تم التبسيط: بنتحقق من الـ Role فقط
        [ProducesResponseType(typeof(Response<ProviderDashboardResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _providerService.GetDashboardAsync(User);
            return StatusCode((int)result.StatusCode, result);
        }

        // ===== PROFILE =====
        /// <summary>
        /// Retrieves the profile details of the current provider.
        /// </summary>
        [HttpGet("profile")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<ProviderProfileResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _providerService.GetProfileAsync(User);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Updates the provider profile information.
        /// </summary>
        [HttpPut("profile")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<ProviderProfileResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProviderProfileRequest request)
        {
            var result = await _providerService.UpdateProfileAsync(request, User);
            return StatusCode((int)result.StatusCode, result);
        }

        // ===== AVAILABILITY =====
        /// <summary>
        /// Toggles provider availability (متاح / غير متاح).
        /// </summary>
        [HttpPut("profile/availability")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> ToggleAvailability([FromBody] ToggleAvailabilityRequest request)
        {
            var result = await _providerService.ToggleAvailabilityAsync(request, User);
            return StatusCode((int)result.StatusCode, result);
        }

        // ===== WORKING AREAS =====
        [HttpGet("working-areas")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetWorkingAreas()
        {
            var result = await _providerService.GetWorkingAreasAsync(User);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("working-areas")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> AddWorkingArea([FromBody] AddWorkingAreaRequest request)
        {
            var result = await _providerService.AddWorkingAreaAsync(request, User);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("working-areas/{id}")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> DeleteWorkingArea(Guid id)
        {
            var result = await _providerService.DeleteWorkingAreaAsync(id, User);
            return StatusCode((int)result.StatusCode, result);
        }

        // ===== AVAILABILITY CALENDAR =====
        [HttpGet("availability")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetAvailabilityCalendar([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddMonths(1);
            var result = await _providerService.GetAvailabilityCalendarAsync(start, end, User);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("availability")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityRequest request)
        {
            var result = await _providerService.AddAvailabilityAsync(request, User);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("availability/{id}")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdateAvailability(Guid id, [FromBody] UpdateAvailabilityRequest request)
        {
            request.Id = id;
            var result = await _providerService.UpdateAvailabilityAsync(request, User);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("availability/{id}")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> DeleteAvailability(Guid id)
        {
            var result = await _providerService.DeleteAvailabilityAsync(id, User);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("availability/bulk")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> AddBulkAvailability([FromBody] BulkAvailabilityRequest request)
        {
            var result = await _providerService.AddBulkAvailabilityAsync(request, User);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}