using ElAnis.DataAccess.Services.ServicePricing;
using ElAnis.Entities.DTO.ServicePricing;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Controller for managing service pricing
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePricingController : ControllerBase
    {
        private readonly IServicePricingService _servicePricingService;
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<CreateServicePricingRequest> _createValidator;
        private readonly IValidator<UpdateServicePricingRequest> _updateValidator;
        private readonly IValidator<BulkServicePricingRequest> _bulkValidator;

        public ServicePricingController(
            IServicePricingService servicePricingService,
            ResponseHandler responseHandler,
            IValidator<CreateServicePricingRequest> createValidator,
            IValidator<UpdateServicePricingRequest> updateValidator,
            IValidator<BulkServicePricingRequest> bulkValidator)
        {
            _servicePricingService = servicePricingService;
            _responseHandler = responseHandler;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _bulkValidator = bulkValidator;
        }

        /// <summary>
        /// Get all categories with their pricing (Public)
        /// </summary>
        /// <returns>List of categories with pricing details</returns>
        /// <response code="200">Categories with pricing retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("categories-with-pricing")]
        [ProducesResponseType(typeof(Response<List<CategoryWithPricingResponse>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetAllCategoriesWithPricing()
        {
            var response = await _servicePricingService.GetAllCategoriesWithPricingAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get pricing for a specific category (Public)
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of pricing for the category</returns>
        /// <response code="200">Pricing retrieved successfully</response>
        /// <response code="400">Invalid category ID</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(Response<List<ServicePricingResponse>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetByCategoryId(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

            var response = await _servicePricingService.GetByCategoryIdAsync(categoryId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all active pricing (Public)
        /// </summary>
        /// <returns>List of active pricing</returns>
        /// <response code="200">Active pricing retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(Response<List<ServicePricingResponse>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetActivePricing()
        {
            var response = await _servicePricingService.GetActivePricingAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get pricing by ID (Public)
        /// </summary>
        /// <param name="id">Pricing ID</param>
        /// <returns>Pricing details</returns>
        /// <response code="200">Pricing retrieved successfully</response>
        /// <response code="400">Invalid pricing ID</response>
        /// <response code="404">Pricing not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<ServicePricingResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid pricing ID"));

            var response = await _servicePricingService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Create new pricing for a category (Admin only)
        /// </summary>
        /// <param name="request">Pricing creation request</param>
        /// <returns>Created pricing details</returns>
        /// <response code="201">Pricing created successfully</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin access required</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(Response<ServicePricingResponse>), 201)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> Create([FromBody] CreateServicePricingRequest request)
        {
            if (request == null)
                return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

            ValidationResult validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var response = await _servicePricingService.CreateAsync(request, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Create multiple pricing records for a category at once (Admin only)
        /// </summary>
        /// <param name="request">Bulk pricing creation request</param>
        /// <returns>List of created pricing records</returns>
        /// <response code="201">Pricing records created successfully</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin access required</response>
        /// <response code="404">Category not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("bulk")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(Response<List<ServicePricingResponse>>), 201)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> CreateBulk([FromBody] BulkServicePricingRequest request)
        {
            if (request == null)
                return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

            ValidationResult validationResult = await _bulkValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var response = await _servicePricingService.CreateBulkAsync(request, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Update existing pricing (Admin only)
        /// </summary>
        /// <param name="id">Pricing ID</param>
        /// <param name="request">Pricing update request</param>
        /// <returns>Updated pricing details</returns>
        /// <response code="200">Pricing updated successfully</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin access required</response>
        /// <response code="404">Pricing not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(Response<ServicePricingResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServicePricingRequest request)
        {
            if (id == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid pricing ID"));

            if (request == null)
                return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

            ValidationResult validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var response = await _servicePricingService.UpdateAsync(id, request, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Delete pricing (Admin only)
        /// </summary>
        /// <param name="id">Pricing ID</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Pricing deleted successfully</response>
        /// <response code="400">Invalid pricing ID</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin access required</response>
        /// <response code="404">Pricing not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid pricing ID"));

            var response = await _servicePricingService.DeleteAsync(id, User);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}