// 2. Refactored Category Service using Repository Pattern

using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;
using Microsoft.Extensions.Logging;

namespace ElAnis.DataAccess.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ResponseHandler _responseHandler;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ResponseHandler responseHandler, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        public async Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var sortedCategories = categories.OrderBy(c => c.Name).ToList();

                var categoryDtos = sortedCategories.Select(c => new CategoryDtoResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                  
                    Description = c.Description,
                    Icon = c.Icon,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                }).ToList();

                return _responseHandler.Success(categoryDtos, "Categories retrieved successfully.");
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
                var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();

                var categoryDtos = categories.Select(c => new CategoryDtoResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                 
                    Description = c.Description,
                    Icon = c.Icon,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                }).ToList();

                return _responseHandler.Success(categoryDtos, "Active categories retrieved successfully.");
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
                var category = await _unitOfWork.Categories.GetByIdAsync(id);

                if (category == null)
                    return _responseHandler.NotFound<CategoryDtoResponse>("Category not found");

                var categoryDto = new CategoryDtoResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                  
                    Description = category.Description,
                    Icon = category.Icon,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt
                };

                return _responseHandler.Success(categoryDto, "Category retrieved successfully.");
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
                 
                    Description = request.Description,
                    Icon = request.Icon,
                    IsActive = request.IsActive
                };

                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.CompleteAsync();

                var categoryDto = new CategoryDtoResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                
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
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                    return _responseHandler.NotFound<CategoryDtoResponse>("Category not found");

                category.Name = request.Name;
               
                category.Description = request.Description;
                category.Icon = request.Icon;
                category.IsActive = request.IsActive;

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();

                var categoryDto = new CategoryDtoResponse
                {
                    Id = category.Id,
                    Name = category.Name,
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
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                    return _responseHandler.NotFound<bool>("Category not found");

                // Check if category has any service providers
                var hasProviders = await _unitOfWork.Categories.HasServiceProvidersAsync(id);
                if (hasProviders)
                {
                    // Soft delete - just deactivate
                    category.IsActive = false;
                    _unitOfWork.Categories.Update(category);
                }
                else
                {
                    // Hard delete
                    _unitOfWork.Categories.Delete(category);
                }

                await _unitOfWork.CompleteAsync();
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