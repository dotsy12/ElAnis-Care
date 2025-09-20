using ElAnis.Entities.DTO.Category;
using ElAnis.Entities.Shared.Bases;

namespace ElAnis.DataAccess.Services.Category
{
	public interface ICategoryService
	{
		Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync();
		Task<Response<List<CategoryDtoResponse>>> GetActiveCategoriesAsync();
		Task<Response<CategoryDtoResponse>> GetCategoryByIdAsync(Guid id);
		Task<Response<CategoryDtoResponse>> CreateCategoryAsync(CreateCategoryRequest request);
		Task<Response<CategoryDtoResponse>> UpdateCategoryAsync(Guid id, CreateCategoryRequest request);
		Task<Response<bool>> DeleteCategoryAsync(Guid id);
	}
}
