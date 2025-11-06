using ElAnis.Entities.DTO.ServicePricing;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace ElAnis.DataAccess.Services.ServicePricing
{
    public class ServicePricingService : IServicePricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServicePricingService> _logger;
        private readonly ResponseHandler _responseHandler;

        public ServicePricingService(
            IUnitOfWork unitOfWork,
            ILogger<ServicePricingService> logger,
            ResponseHandler responseHandler)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _responseHandler = responseHandler;
        }

        // ===== CREATE =====
        public async Task<Response<ServicePricingResponse>> CreateAsync(
            CreateServicePricingRequest request,
            ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (category == null)
                    return _responseHandler.NotFound<ServicePricingResponse>("Category not found");

                var exists = await _unitOfWork.ServicePricings
                    .ExistsForShiftTypeAsync(request.CategoryId, request.ShiftType);

                if (exists)
                    return _responseHandler.BadRequest<ServicePricingResponse>(
                        $"Pricing for {GetShiftTypeName(request.ShiftType)} already exists for this category");

                var pricing = new ElAnis.Entities.Models.ServicePricing
                {
                    CategoryId = request.CategoryId,
                    ShiftType = request.ShiftType,
                    PricePerShift = request.PricePerShift,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    UpdatedBy = userId
                };

                await _unitOfWork.ServicePricings.AddAsync(pricing);
                await _unitOfWork.CompleteAsync();

                var response = MapToResponse(pricing, category);
                return _responseHandler.Created(response, "Service pricing created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service pricing");
                return _responseHandler.ServerError<ServicePricingResponse>("Error creating service pricing");
            }
        }

        // ===== CREATE BULK =====
        public async Task<Response<List<ServicePricingResponse>>> CreateBulkAsync(
            BulkServicePricingRequest request,
            ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (category == null)
                    return _responseHandler.NotFound<List<ServicePricingResponse>>("Category not found");

                var duplicateShiftTypes = request.Pricings
                    .GroupBy(p => p.ShiftType)
                    .Where(g => g.Count() > 1)
                    .Select(g => GetShiftTypeName(g.Key))
                    .ToList();

                if (duplicateShiftTypes.Any())
                    return _responseHandler.BadRequest<List<ServicePricingResponse>>(
                        $"Duplicate shift types found: {string.Join(", ", duplicateShiftTypes)}");

                var existingPricings = await _unitOfWork.ServicePricings
                    .GetByCategoryIdAsync(request.CategoryId);

                var existingShiftTypes = existingPricings.Select(p => p.ShiftType).ToHashSet();
                var conflictingTypes = request.Pricings
                    .Where(p => existingShiftTypes.Contains(p.ShiftType))
                    .Select(p => GetShiftTypeName(p.ShiftType))
                    .ToList();

                if (conflictingTypes.Any())
                    return _responseHandler.BadRequest<List<ServicePricingResponse>>(
                        $"Pricing already exists for: {string.Join(", ", conflictingTypes)}");

                var newPricings = request.Pricings.Select(p => new ElAnis.Entities.Models.ServicePricing
                {
                    CategoryId = request.CategoryId,
                    ShiftType = p.ShiftType,
                    PricePerShift = p.PricePerShift,
                    Description = p.Description,
                    IsActive = true,
                    UpdatedBy = userId
                }).ToList();

                await _unitOfWork.ServicePricings.AddRangeAsync(newPricings);
                await _unitOfWork.CompleteAsync();

                var responses = newPricings.Select(p => MapToResponse(p, category)).ToList();
                return _responseHandler.Created(responses, $"{responses.Count} pricing records created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk service pricing");
                return _responseHandler.ServerError<List<ServicePricingResponse>>("Error creating bulk service pricing");
            }
        }

        // ===== UPDATE =====
        public async Task<Response<ServicePricingResponse>> UpdateAsync(
            Guid id,
            UpdateServicePricingRequest request,
            ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var pricing = await _unitOfWork.ServicePricings.GetByIdAsync(id);
                if (pricing == null)
                    return _responseHandler.NotFound<ServicePricingResponse>("Service pricing not found");

                var category = await _unitOfWork.Categories.GetByIdAsync(pricing.CategoryId);
                if (category == null)
                    return _responseHandler.NotFound<ServicePricingResponse>("Category not found");

                pricing.PricePerShift = request.PricePerShift;
                pricing.Description = request.Description;
                pricing.IsActive = request.IsActive;
                pricing.UpdatedAt = DateTime.UtcNow;
                pricing.UpdatedBy = userId;

                _unitOfWork.ServicePricings.Update(pricing);
                await _unitOfWork.CompleteAsync();

                var response = MapToResponse(pricing, category);
                return _responseHandler.Success(response, "Service pricing updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service pricing");
                return _responseHandler.ServerError<ServicePricingResponse>("Error updating service pricing");
            }
        }

        // ===== DELETE =====
        public async Task<Response<string>> DeleteAsync(Guid id, ClaimsPrincipal userClaims)
        {
            try
            {
                var pricing = await _unitOfWork.ServicePricings.GetByIdAsync(id);
                if (pricing == null)
                    return _responseHandler.NotFound<string>("Service pricing not found");

                pricing.IsActive = false;
                pricing.UpdatedAt = DateTime.UtcNow;
                pricing.UpdatedBy = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                _unitOfWork.ServicePricings.Update(pricing);
                await _unitOfWork.CompleteAsync();

                return _responseHandler.Success<string>(null, "Service pricing deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service pricing");
                return _responseHandler.ServerError<string>("Error deleting service pricing");
            }
        }

        // ===== GET BY ID =====
        public async Task<Response<ServicePricingResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var pricing = await _unitOfWork.ServicePricings
                    .GetQueryable()
                    .Where(p => p.Id == id)
                    .Select(p => new ServicePricingResponse
                    {
                        Id = p.Id,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category.Name,
                        ShiftType = p.ShiftType,
                        ShiftTypeName = GetShiftTypeName(p.ShiftType),
                        PricePerShift = p.PricePerShift,
                        Description = p.Description,
                        IsActive = p.IsActive,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt,
                        UpdatedBy = p.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                if (pricing == null)
                    return _responseHandler.NotFound<ServicePricingResponse>("Service pricing not found");

                return _responseHandler.Success(pricing, "Service pricing retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service pricing");
                return _responseHandler.ServerError<ServicePricingResponse>("Error retrieving service pricing");
            }
        }

        // ===== GET BY CATEGORY =====
        public async Task<Response<List<ServicePricingResponse>>> GetByCategoryIdAsync(Guid categoryId)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category == null)
                    return _responseHandler.NotFound<List<ServicePricingResponse>>("Category not found");

                var pricings = await _unitOfWork.ServicePricings.GetByCategoryIdAsync(categoryId);
                var responses = pricings.Select(p => MapToResponse(p, category)).ToList();

                return _responseHandler.Success(responses, "Pricing retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pricing by category");
                return _responseHandler.ServerError<List<ServicePricingResponse>>("Error retrieving pricing");
            }
        }

        // ===== GET ALL CATEGORIES WITH PRICING =====
        public async Task<Response<List<CategoryWithPricingResponse>>> GetAllCategoriesWithPricingAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var pricingsDict = await _unitOfWork.ServicePricings.GetAllCategoriesWithPricingAsync();

                var responses = categories.Select(c => new CategoryWithPricingResponse
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    CategoryDescription = c.Description,
                    CategoryIcon = c.Icon,
                    CategoryIsActive = c.IsActive,
                    Pricing = pricingsDict.ContainsKey(c.Id)
                        ? pricingsDict[c.Id].Select(p => MapToResponse(p, c)).ToList()
                        : new List<ServicePricingResponse>()
                }).ToList();

                return _responseHandler.Success(responses, "Categories with pricing retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories with pricing");
                return _responseHandler.ServerError<List<CategoryWithPricingResponse>>("Error retrieving data");
            }
        }

        // ===== GET ACTIVE PRICING =====
        public async Task<Response<List<ServicePricingResponse>>> GetActivePricingAsync()
        {
            try
            {
                var pricings = await _unitOfWork.ServicePricings.GetActivePricingAsync();
                var responses = pricings.Select(p => MapToResponse(p, p.Category)).ToList();

                return _responseHandler.Success(responses, "Active pricing retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active pricing");
                return _responseHandler.ServerError<List<ServicePricingResponse>>("Error retrieving active pricing");
            }
        }

        // ===== HELPER METHODS =====
        private ServicePricingResponse MapToResponse(ElAnis.Entities.Models.ServicePricing pricing, ElAnis.Entities.Models.Category category)
        {
            return new ServicePricingResponse
            {
                Id = pricing.Id,
                CategoryId = pricing.CategoryId,
                CategoryName = category.Name,
                ShiftType = pricing.ShiftType,
                ShiftTypeName = GetShiftTypeName(pricing.ShiftType),
                PricePerShift = pricing.PricePerShift,
                Description = pricing.Description,
                IsActive = pricing.IsActive,
                CreatedAt = pricing.CreatedAt,
                UpdatedAt = pricing.UpdatedAt,
                UpdatedBy = pricing.UpdatedBy
            };
        }

        private string GetShiftTypeName(ShiftType shiftType)
        {
            return shiftType switch
            {
                ShiftType.ThreeHours => "3 Hours",
                ShiftType.TwelveHours => "12 Hours",
                ShiftType.TwentyFourHours => "24 Hours",
                _ => "Unknown"
            };
        }
    }
}
