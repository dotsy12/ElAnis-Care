using ElAnis.DataAccess.Services.Category;
using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
	/// <summary>
	/// Controller for managing categories
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;
		private readonly ResponseHandler _responseHandler;
		private readonly IValidator<CreateCategoryRequest> _createCategoryValidator;
		private readonly IValidator<CreateCategoryRequest> _updateCategoryValidator;

		/// <summary>
		/// Initializes a new instance of CategoryController
		/// </summary>
		/// <param name="categoryService">The category service</param>
		/// <param name="responseHandler">Response handler for consistent API responses</param>
		/// <param name="createCategoryValidator">Validator for create category requests</param>
		/// <param name="updateCategoryValidator">Validator for update category requests</param>
		public CategoryController(
			ICategoryService categoryService,
			ResponseHandler responseHandler,
			IValidator<CreateCategoryRequest> createCategoryValidator,
			IValidator<CreateCategoryRequest> updateCategoryValidator = null)
		{
			_categoryService = categoryService;
			_responseHandler = responseHandler;
			_createCategoryValidator = createCategoryValidator;
			_updateCategoryValidator = updateCategoryValidator ?? createCategoryValidator;
		}

		/// <summary>
		/// Retrieves all categories including inactive ones
		/// </summary>
		/// <returns>List of all categories</returns>
		/// <response code="200">Categories retrieved successfully</response>
		/// <response code="500">Internal server error</response>
		[HttpGet]
		[ProducesResponseType(typeof(Response<IEnumerable<CategoryDtoResponse>>), 200)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> GetAllCategories()
		{
			var response = await _categoryService.GetAllCategoriesAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Retrieves only active categories
		/// </summary>
		/// <returns>List of active categories</returns>
		/// <response code="200">Active categories retrieved successfully</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("active")]
		[ProducesResponseType(typeof(Response<IEnumerable<CategoryDtoResponse>>), 200)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> GetActiveCategories()
		{
			var response = await _categoryService.GetActiveCategoriesAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Retrieves a specific category by its ID
		/// </summary>
		/// <param name="id">The unique identifier of the category</param>
		/// <returns>Category details</returns>
		/// <response code="200">Category found and returned successfully</response>
		/// <response code="400">Invalid category ID provided</response>
		/// <response code="404">Category not found</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(Response<CategoryDtoResponse>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> GetCategoryById(Guid id)
		{
			// التحقق من صحة Guid
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

			var response = await _categoryService.GetCategoryByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Creates a new category (Admin only)
		/// </summary>
		/// <param name="request">The category creation request</param>
		/// <returns>Created category details</returns>
		/// <response code="201">Category created successfully</response>
		/// <response code="400">Invalid request data or validation errors</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="500">Internal server error</response>
		[HttpPost]
		[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(typeof(Response<CategoryDtoResponse>), 201)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
		{
			// التحقق من وجود البيانات
			if (request == null)
				return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

			ValidationResult validationResult = await _createCategoryValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BadRequest(_responseHandler.BadRequest<object>(errors));
			}

			var response = await _categoryService.CreateCategoryAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Updates an existing category (Admin only)
		/// </summary>
		/// <param name="id">The unique identifier of the category to update</param>
		/// <param name="request">The category update request</param>
		/// <returns>Updated category details</returns>
		/// <response code="200">Category updated successfully</response>
		/// <response code="400">Invalid request data, category ID, or validation errors</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="404">Category not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPut("{id}")]
		[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(typeof(Response<CategoryDtoResponse>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateCategoryRequest request)
		{
			// التحقق من صحة Guid
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

			// التحقق من وجود البيانات
			if (request == null)
				return BadRequest(_responseHandler.BadRequest<object>("Request cannot be null"));

			// استخدام FluentValidation بدلاً من ModelState
			ValidationResult validationResult = await _updateCategoryValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BadRequest(_responseHandler.BadRequest<object>(errors));
			}

			var response = await _categoryService.UpdateCategoryAsync(id, request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Deletes a category (Admin only)
		/// </summary>
		/// <param name="id">The unique identifier of the category to delete</param>
		/// <returns>Deletion confirmation</returns>
		/// <response code="200">Category deleted successfully</response>
		/// <response code="400">Invalid category ID provided</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="404">Category not found</response>
		/// <response code="409">Conflict - Category cannot be deleted (may have dependencies)</response>
		/// <response code="500">Internal server error</response>
		[HttpDelete("{id}")]
		[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 409)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> DeleteCategory(Guid id)
		{
			// التحقق من صحة Guid
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

			var response = await _categoryService.DeleteCategoryAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}