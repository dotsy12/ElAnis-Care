using ElAnis.DataAccess.Services.ServiceRequest;
using ElAnis.Entities.DTO.ServiceRequest;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Controller for managing service requests (user side and provider side)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestsController : ControllerBase
    {
        private readonly IServiceRequestService _requestService;
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<CreateServiceRequestDto> _createValidator;
        private readonly IValidator<ProviderResponseDto> _responseValidator;

        public RequestsController(
            IServiceRequestService requestService,
            ResponseHandler responseHandler,
            IValidator<CreateServiceRequestDto> createValidator,
            IValidator<ProviderResponseDto> responseValidator)
        {
            _requestService = requestService;
            _responseHandler = responseHandler;
            _createValidator = createValidator;
            _responseValidator = responseValidator;
        }

        /// <summary>
        /// Create a new service request
        /// </summary>
        /// <param name="request">Request creation data</param>
        /// <returns>Created service request</returns>
        /// <response code="201">Request created successfully</response>
        /// <response code="400">Invalid request data or validation errors</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="404">Provider or category not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Response<ServiceRequestResponse>), 201)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> CreateRequest([FromBody] CreateServiceRequestDto request)
        {
            if (request == null)
                return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

            ValidationResult validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var response = await _requestService.CreateRequestAsync(request, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all requests for the authenticated user
        /// </summary>
        /// <returns>List of user's service requests</returns>
        /// <response code="200">Requests retrieved successfully</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("user")]
        [ProducesResponseType(typeof(Response<List<ServiceRequestResponse>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetUserRequests()
        {
            var response = await _requestService.GetUserRequestsAsync(User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get all requests for a specific provider (Provider only)
        /// </summary>
        /// <param name="providerId">Provider ID</param>
        /// <returns>List of provider's service requests</returns>
        /// <response code="200">Requests retrieved successfully</response>
        /// <response code="400">Invalid provider ID</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="404">Provider not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("provider/{providerId}")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<List<ServiceRequestResponse>>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetProviderRequests(Guid providerId)
        {
            if (providerId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid provider ID"));

            var response = await _requestService.GetProviderRequestsAsync(providerId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Provider accepts or rejects a service request (Provider only)
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <param name="response">Provider's response (Accepted/Rejected with optional reason)</param>
        /// <returns>Updated service request</returns>
        /// <response code="200">Response submitted successfully</response>
        /// <response code="400">Invalid request data or request not in pending state</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Not authorized to respond to this request</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{requestId}/response")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<ServiceRequestResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> RespondToRequest(Guid requestId, [FromBody] ProviderResponseDto response)
        {
            if (requestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

            if (response == null)
                return BadRequest(_responseHandler.BadRequest<object>("Response cannot be null"));

            ValidationResult validationResult = await _responseValidator.ValidateAsync(response);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return BadRequest(_responseHandler.BadRequest<object>(errors));
            }

            var result = await _requestService.RespondToRequestAsync(requestId, response, User);
            return StatusCode((int)result.StatusCode, result);
        }



        /// <summary>
        /// Provider starts the service (changes status from Paid to InProgress)
        /// </summary>
        /// <param name="requestId">Service request ID</param>
        /// <returns>Updated service request</returns>
        /// <response code="200">Service started successfully</response>
        /// <response code="400">Invalid request ID or request not in Paid status</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Not authorized to start this request</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{requestId}/start")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<ServiceRequestResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> StartRequest(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

            var result = await _requestService.StartRequestAsync(requestId, User);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Provider completes the service (changes status from InProgress to Completed)
        /// </summary>
        /// <param name="requestId">Service request ID</param>
        /// <returns>Updated service request with completion timestamp</returns>
        /// <response code="200">Service completed successfully</response>
        /// <response code="400">Invalid request ID or request not in InProgress status</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Not authorized to complete this request</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{requestId}/complete")]
        [Authorize(Roles = "Provider")]
        [ProducesResponseType(typeof(Response<ServiceRequestResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> CompleteRequest(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

            var result = await _requestService.CompleteRequestAsync(requestId, User);
            return StatusCode((int)result.StatusCode, result);
        }
    }


}