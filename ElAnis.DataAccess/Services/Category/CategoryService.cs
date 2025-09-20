using ElAnis.DataAccess.ApplicationContext;
using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Models;
using ElAnis.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElAnis.DataAccess.Services.Category
{
	public class CategoryService : ICategoryService
	{
		private readonly AuthContext _context;
		private readonly ResponseHandler _responseHandler;
		private readonly ILogger<CategoryService> _logger;

		public CategoryService(AuthContext context, ResponseHandler responseHandler, ILogger<CategoryService> logger)
		{
			_context = context;
			_responseHandler = responseHandler;
			_logger = logger;
		}

		public async Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync()
		{
			try
			{
				var categories = await _context.Categories
					.OrderBy(c => c.Name)
					.Select(c => new CategoryDtoResponse
					{
						Id = c.Id,
						Name = c.Name,
						NameEn = c.NameEn,
						Description = c.Description,
						Icon = c.Icon,
						IsActive = c.IsActive,
						CreatedAt = c.CreatedAt
					})
					.ToListAsync();

				return _responseHandler.Success(categories, "Categories retrieved successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving categories");
				return _responseHandler.ServerError<List<CategoryDtoResponse>>("Error retrieving categories");
			}
		}

		public async Task<Response<List<CategoryDtoResponse>>> GetActiveCategoriesAsync()
		{
			try
			{
				var categories = await _context.Categories
					.Where(c => c.IsActive)
					.OrderBy(c => c.Name)
					.Select(c => new CategoryDtoResponse
					{
						Id = c.Id,
						Name = c.Name,
						NameEn = c.NameEn,
						Description = c.Description,
						Icon = c.Icon,
						IsActive = c.IsActive,
						CreatedAt = c.CreatedAt
					})
					.ToListAsync();

				return _responseHandler.Success(categories, "Active categories retrieved successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving active categories");
				return _responseHandler.ServerError<List<CategoryDtoResponse>>("Error retrieving categories");
			}
		}

		public async Task<Response<CategoryDtoResponse>> GetCategoryByIdAsync(Guid id)
		{
			try
			{
				var category = await _context.Categories
					.Where(c => c.Id == id)
					.Select(c => new CategoryDtoResponse
					{
						Id = c.Id,
						Name = c.Name,
						NameEn = c.NameEn,
						Description = c.Description,
						Icon = c.Icon,
						IsActive = c.IsActive,
						CreatedAt = c.CreatedAt
					})
					.FirstOrDefaultAsync();

				if (category == null)
					return _responseHandler.NotFound<CategoryDtoResponse>("Category not found");

				return _responseHandler.Success(category, "Category retrieved successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving category with ID: {CategoryId}", id);
				return _responseHandler.ServerError<CategoryDtoResponse>("Error retrieving category");
			}
		}

		public async Task<Response<CategoryDtoResponse>> CreateCategoryAsync(CreateCategoryRequest request)
		{
			try
			{
				var category = new ElAnis.Entities.Models.Category
				{
					Name = request.Name,
					NameEn = request.NameEn,
					Description = request.Description,
					Icon = request.Icon,
					IsActive = request.IsActive
				};

				_context.Categories.Add(category);
				await _context.SaveChangesAsync();

				var categoryDto = new CategoryDtoResponse
				{
					Id = category.Id,
					Name = category.Name,
					NameEn = category.NameEn,
					Description = category.Description,
					Icon = category.Icon,
					IsActive = category.IsActive,
					CreatedAt = category.CreatedAt
				};

				return _responseHandler.Created(categoryDto, "Category created successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating category");
				return _responseHandler.ServerError<CategoryDtoResponse>("Error creating category");
			}
		}

		public async Task<Response<CategoryDtoResponse>> UpdateCategoryAsync(Guid id, CreateCategoryRequest request)
		{
			try
			{
				var category = await _context.Categories.FindAsync(id);
				if (category == null)
					return _responseHandler.NotFound<CategoryDtoResponse>("Category not found");

				category.Name = request.Name;
				category.NameEn = request.NameEn;
				category.Description = request.Description;
				category.Icon = request.Icon;
				category.IsActive = request.IsActive;

				await _context.SaveChangesAsync();

				var categoryDto = new CategoryDtoResponse
				{
					Id = category.Id,
					Name = category.Name,
					NameEn = category.NameEn,
					Description = category.Description,
					Icon = category.Icon,
					IsActive = category.IsActive,
					CreatedAt = category.CreatedAt
				};

				return _responseHandler.Success(categoryDto, "Category updated successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating category with ID: {CategoryId}", id);
				return _responseHandler.ServerError<CategoryDtoResponse>("Error updating category");
			}
		}

		public async Task<Response<bool>> DeleteCategoryAsync(Guid id)
		{
			try
			{
				var category = await _context.Categories.FindAsync(id);
				if (category == null)
					return _responseHandler.NotFound<bool>("Category not found");

				// Check if category has any service providers
				var hasProviders = await _context.ServiceProviderCategories.AnyAsync(spc => spc.CategoryId == id);
				if (hasProviders)
				{
					// Soft delete - just deactivate
					category.IsActive = false;
				}
				else
				{
					// Hard delete
					_context.Categories.Remove(category);
				}

				await _context.SaveChangesAsync();
				return _responseHandler.Success(true, "Category deleted successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting category with ID: {CategoryId}", id);
				return _responseHandler.ServerError<bool>("Error deleting category");
			}
		}

	
	}
}