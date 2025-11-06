using ElAnis.Entities.DTO.ServicePricing;
using ElAnis.Entities.Shared.Bases;

using System.Security.Claims;


namespace ElAnis.DataAccess.Services.ServicePricing
{
 //===== IServicePricingService.cs =====
    public interface IServicePricingService
    {
        // CRUD Operations
        Task<Response<ServicePricingResponse>> CreateAsync(CreateServicePricingRequest request, ClaimsPrincipal userClaims);
        Task<Response<List<ServicePricingResponse>>> CreateBulkAsync(BulkServicePricingRequest request, ClaimsPrincipal userClaims);
        Task<Response<ServicePricingResponse>> UpdateAsync(Guid id, UpdateServicePricingRequest request, ClaimsPrincipal userClaims);
        Task<Response<string>> DeleteAsync(Guid id, ClaimsPrincipal userClaims);

        // Queries
        Task<Response<ServicePricingResponse>> GetByIdAsync(Guid id);
        Task<Response<List<ServicePricingResponse>>> GetByCategoryIdAsync(Guid categoryId);
        Task<Response<List<CategoryWithPricingResponse>>> GetAllCategoriesWithPricingAsync();
        Task<Response<List<ServicePricingResponse>>> GetActivePricingAsync();
    }

}
