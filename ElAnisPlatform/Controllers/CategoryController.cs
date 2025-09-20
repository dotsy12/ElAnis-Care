using ElAnis.DataAccess.Services.Category;
using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;
		private readonly ResponseHandler _responseHandler;
		private readonly IValidator<CreateCategoryRequest> _createCategoryValidator;
		private readonly IValidator<CreateCategoryRequest> _updateCategoryValidator;

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

		[HttpGet]
		public async Task<IActionResult> GetAllCategories()
		{
			var response = await _categoryService.GetAllCategoriesAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpGet("active")]
		public async Task<IActionResult> GetActiveCategories()
		{
			var response = await _categoryService.GetActiveCategoriesAsync();
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetCategoryById(Guid id)
		{
			// التحقق من صحة Guid
			if (id == Guid.Empty)
				return BadRequest(_responseHandler.BadRequest<object>("Invalid category ID"));

			var response = await _categoryService.GetCategoryByIdAsync(id);
			return StatusCode((int)response.StatusCode, response);
		}

		[HttpPost]
		[Authorize(Policy = "AdminOnly")]
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

		[HttpPut("{id}")]
		[Authorize(Policy = "AdminOnly")]
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

		[HttpDelete("{id}")]
		[Authorize(Policy = "AdminOnly")]
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