## 📌 Database Architecture Description

The project uses a relational database (likely SQL Server). Data is structured in normalized tables representing core domain entities.


## 📌 Key Entities & Relationships

### 🔸 CategoryConfiguration
No explicit relationships found.

### 🔸 NotificationConfiguration
No explicit relationships found.

### 🔸 PaymentConfiguration
No explicit relationships found.

### 🔸 ProviderAvailabilityConfiguration
No explicit relationships found.

### 🔸 ProviderWorkingAreaConfiguration
No explicit relationships found.

### 🔸 ReviewEntityConfiguration
No explicit relationships found.

### 🔸 ServicePricingConfiguration
No explicit relationships found.

### 🔸 ServiceProviderApplicationConfiguration
No explicit relationships found.

### 🔸 ServiceProviderCategoryConfiguration
No explicit relationships found.

### 🔸 ServiceProviderApplicationEntityConfiguration
No explicit relationships found.

### 🔸 ServiceRequestConfiguration
No explicit relationships found.

### 🔸 UserConfiguration
No explicit relationships found.

### 🔸 RefreshTokenResponse
No explicit relationships found.

### 🔸 GoogleLoginRequest
No explicit relationships found.

### 🔸 LoginRequest
No explicit relationships found.

### 🔸 LoginResponse
No explicit relationships found.

### 🔸 AdminRegisterRequest
No explicit relationships found.

### 🔸 GoogleRegisterResponse
- Has a foreign key reference to **string**.

### 🔸 RegisterRequest
No explicit relationships found.

### 🔸 RegisterResponse
No explicit relationships found.

### 🔸 RegisterServiceProviderRequest
- Has a foreign key reference to **string**.
- One-to-Many relationship with **Guid**.

### 🔸 ResendOtpRequest
- Has a foreign key reference to **string**.

### 🔸 ServiceProviderApplicationResponse
- Has a foreign key reference to **Guid**.
- Has a foreign key reference to **string**.

### 🔸 VerifyOtpRequest
- Has a foreign key reference to **string**.

### 🔸 ChangePasswordRequest
No explicit relationships found.

### 🔸 ForgetPasswordRequest
No explicit relationships found.

### 🔸 ForgetPasswordResponse
- Has a foreign key reference to **string**.

### 🔸 ResetPasswordRequest
- Has a foreign key reference to **string**.

### 🔸 ResetPasswordResponse
- Has a foreign key reference to **string**.

### 🔸 AdminDashboardStatsDto
No explicit relationships found.

### 🔸 PaginatedResult
- One-to-Many relationship with **T**.

### 🔸 PaymentTransactionDto
- Has a foreign key reference to **string**.
- Has a foreign key reference to **Guid**.
- One-to-Many relationship with **PaymentTransactionDto**.

### 🔸 RecentBookingDto
No explicit relationships found.

### 🔸 RejectApplicationRequest
No explicit relationships found.

### 🔸 ServiceProviderApplicationDetailDto
- Has a foreign key reference to **string**.
- Has a foreign key reference to **string**.
- One-to-Many relationship with **Guid**.

### 🔸 ServiceProviderApplicationDto
- Has a foreign key reference to **string**.

### 🔸 ServiceProviderDto
- Has a foreign key reference to **string**.

### 🔸 SuspendServiceProviderRequest
No explicit relationships found.

### 🔸 UserManagementDto
No explicit relationships found.

### 🔸 AddAvailabilityRequest
No explicit relationships found.

### 🔸 AvailabilityCalendarResponse
- One-to-Many relationship with **AvailabilityDto**.
- One-to-Many relationship with **ServiceRequestSummary**.

### 🔸 AvailabilityDto
No explicit relationships found.

### 🔸 BulkAvailabilityRequest
- One-to-Many relationship with **DayOfWeek**.

### 🔸 UpdateAvailabilityRequest
No explicit relationships found.

### 🔸 CategoryDtoResponse
No explicit relationships found.

### 🔸 CreateCategoryRequest
No explicit relationships found.

### 🔸 CreatePaymentDto
- Has a foreign key reference to **Guid**.

### 🔸 PaymentResponse
- Has a foreign key reference to **Guid**.

### 🔸 GetProvidersRequest
No explicit relationships found.

### 🔸 ProviderDetailResponse
- Has a foreign key reference to **Guid**.
- Has a foreign key reference to **Guid**.
- One-to-Many relationship with **CategoryDto**.
- One-to-Many relationship with **ProviderWorkingAreaDto**.
- One-to-Many relationship with **AvailabilityDto**.
- One-to-Many relationship with **ShiftPriceDto**.

### 🔸 ProviderSummaryResponse
- One-to-Many relationship with **CategoryDto**.

### 🔸 CreateServiceRequestDto
- Has a foreign key reference to **Guid**.

### 🔸 ProviderResponseDto
No explicit relationships found.

### 🔸 ServiceRequestResponse
- Has a foreign key reference to **Guid**.

### 🔸 CreateReviewDto
- Has a foreign key reference to **Guid**.

### 🔸 ProviderReviewsResponse
- Has a foreign key reference to **Guid**.
- One-to-Many relationship with **ReviewResponse**.

### 🔸 ReviewResponse
- Has a foreign key reference to **Guid**.

### 🔸 BulkServicePricingRequest
- Has a foreign key reference to **Guid**.
- One-to-Many relationship with **PricingItem**.

### 🔸 CategoryWithPricingResponse
- Has a foreign key reference to **Guid**.
- One-to-Many relationship with **ServicePricingResponse**.

### 🔸 CreateServicePricingRequest
- Has a foreign key reference to **Guid**.

### 🔸 ServicePricingResponse
- Has a foreign key reference to **Guid**.

### 🔸 UpdateServicePricingRequest
No explicit relationships found.

### 🔸 ApplicationStatusResponse
- Has a foreign key reference to **Guid**.

### 🔸 ProviderDashboardResponse
- Has a foreign key reference to **Guid**.
- One-to-Many relationship with **ServiceRequestSummary**.
- One-to-Many relationship with **ServiceRequestSummary**.
- One-to-Many relationship with **CategorySummary**.
- One-to-Many relationship with **string**.

### 🔸 ProviderProfileResponse
- Has a foreign key reference to **string**.
- One-to-Many relationship with **CategorySummary**.
- One-to-Many relationship with **WorkingAreaDto**.

### 🔸 ToggleAvailabilityRequest
No explicit relationships found.

### 🔸 UpdateProviderProfileRequest
No explicit relationships found.

### 🔸 UpdateProfilePictureRequest
No explicit relationships found.

### 🔸 UploadResultDto
- Has a foreign key reference to **string**.

### 🔸 AddWorkingAreaRequest
No explicit relationships found.

### 🔸 UpdateWorkingAreaRequest
No explicit relationships found.

### 🔸 WorkingAreaDto
No explicit relationships found.

### 🔸 Category
- One-to-Many relationship with **ServicePricing**.
- One-to-Many relationship with **ServiceProviderCategory**.
- One-to-Many relationship with **ServiceRequest**.

### 🔸 Notification
- Has a foreign key reference to **string**.

### 🔸 Payment
- Has a foreign key reference to **Guid**.

### 🔸 ProviderAvailability
- Has a foreign key reference to **Guid**.

### 🔸 ProviderWorkingArea
- Has a foreign key reference to **Guid**.

### 🔸 Review
- Has a foreign key reference to **string**.
- Has a foreign key reference to **string**.
- Has a foreign key reference to **Guid**.

### 🔸 ServicePricing
- Has a foreign key reference to **Guid**.

### 🔸 ServiceProviderApplication
- Has a foreign key reference to **string**.
- Has a foreign key reference to **string**.
- One-to-Many relationship with **Guid**.

### 🔸 ServiceProviderCategory
- Has a foreign key reference to **Guid**.
- Has a foreign key reference to **Guid**.

### 🔸 ServiceProviderProfile
- Has a foreign key reference to **string**.
- One-to-Many relationship with **ServiceProviderCategory**.
- One-to-Many relationship with **ProviderWorkingArea**.
- One-to-Many relationship with **ProviderAvailability**.
- One-to-Many relationship with **ServiceRequest**.
- One-to-Many relationship with **Review**.

### 🔸 ServiceRequest
- Has a foreign key reference to **string**.
- Has a foreign key reference to **Guid**.

### 🔸 Role
No explicit relationships found.

### 🔸 User
- One-to-Many relationship with **ServiceRequest**.
- One-to-Many relationship with **Review**.
- One-to-Many relationship with **Review**.
- One-to-Many relationship with **Notification**.

### 🔸 UserRefreshToken
- Has a foreign key reference to **string**.

### 🔸 Response
- One-to-Many relationship with **string**.

### 🔸 ResponseHandler
No explicit relationships found.


## 📌 Data Flow Description


1. **Data Collection**  
   Data is received from user requests through Controllers, validated, and passed to Services/Repositories.

2. **Data Storage**  
   EF Core maps entities to database tables using the DbContext via DbSet<> collections.

3. **Data Access**  
   CRUD operations are executed through EF Core.  
   Navigation properties allow fetching related data automatically via lazy or eager loading.
