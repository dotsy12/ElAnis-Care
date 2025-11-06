using System.Security.Claims;
using ElAnis.DataAccess.Services.Email;
using ElAnis.DataAccess.Services.OTP;
using ElAnis.DataAccess.Services.Token;
using ElAnis.Entities.DTO.Account.Auth.Login;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Account.Auth.ResetPassword;
using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models.Auth.UserTokens;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using LoginRequest = ElAnis.Entities.DTO.Account.Auth.Login.LoginRequest;
using ResetPasswordRequest = ElAnis.Entities.DTO.Account.Auth.ResetPassword.ResetPasswordRequest;
using RefreshTokenResponse = ElAnis.Entities.DTO.Account.Auth.RefreshTokenResponse;

namespace ElAnis.DataAccess.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IOTPService _otpService;
        private readonly ResponseHandler _responseHandler;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IOTPService otpService,
            ResponseHandler responseHandler,
            ITokenStoreService tokenStoreService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _otpService = otpService;
            _responseHandler = responseHandler;
            _tokenStoreService = tokenStoreService;
            _logger = logger;
        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            // Find user by email or phone number using repository
            User? user = await _unitOfWork.Users.FindByEmailOrPhoneAsync(loginRequest.Email, loginRequest.PhoneNumber);

            if (user == null)
                return _responseHandler.NotFound<LoginResponse>("User not found.");

            // Check password
            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return _responseHandler.BadRequest<LoginResponse>("Invalid password.");

            // Check if email is confirmed
            if (!user.EmailConfirmed)
                return _responseHandler.BadRequest<LoginResponse>("Email is not verified. Please verify your email first.");

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate tokens
            var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

            // إنشاء الريسبونس الأساسي
            var response = new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault(),
                IsEmailConfirmed = user.EmailConfirmed,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            };

            if (roles.Contains("Provider"))
            {
                var application = await _unitOfWork.ServiceProviderApplications.GetByUserIdAsync(user.Id);

                if (application != null)
                {
                    response.ProviderStatus = application.Status; // الحالة من الأبلكيشن مش البروفايل
                }
            }


            return _responseHandler.Success(response, "Login successful.");
          
        }

        public async Task<Response<ForgetPasswordResponse>> ForgotPasswordAsync(ForgetPasswordRequest model)
        {
            _logger.LogInformation("Starting ForgotPasswordAsync for Email: {Email}, Phone: {Phone}", model.Email, model.PhoneNumber);

            // Find user by email or phone number using repository
            User? user = await _unitOfWork.Users.FindByEmailOrPhoneAsync(model.Email, model.PhoneNumber);

            if (user == null)
            {
                _logger.LogWarning("User not found for Email: {Email}, Phone: {Phone}", model.Email, model.PhoneNumber);
                return _responseHandler.NotFound<ForgetPasswordResponse>("User not found.");
            }

            // Generate and send OTP
            _logger.LogInformation("User found with ID: {UserId}. Generating OTP...", user.Id);
            var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

            try
            {
                await _emailService.SendOtpEmailAsync(user, otp);
                _logger.LogInformation("OTP sent successfully to user ID: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to user ID: {UserId}", user.Id);
            }

            var response = new ForgetPasswordResponse
            {
                UserId = user.Id
            };

            return _responseHandler.Success(response, "OTP sent to your email. Please use it to reset your password.");
        }

        public async Task<Response<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest model)
        {
            _logger.LogInformation("Starting ResetPasswordAsync for User ID: {UserId}", model.UserId);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", model.UserId);
                return _responseHandler.NotFound<ResetPasswordResponse>("User not found.");
            }

            // Verify OTP
            _logger.LogInformation("Validating OTP for User ID: {UserId}", user.Id);
            var isOtpValid = await _otpService.ValidateOtpAsync(model.UserId, model.Otp);
            if (!isOtpValid)
            {
                _logger.LogWarning("Invalid or expired OTP for User ID: {UserId}", model.UserId);
                return _responseHandler.BadRequest<ResetPasswordResponse>("Invalid or expired OTP.");
            }

            _logger.LogInformation("OTP is valid. Proceeding to reset password for User ID: {UserId}", user.Id);

            // Reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                _logger.LogWarning("Password reset failed for User ID: {UserId}. Errors: {Errors}", user.Id, string.Join(", ", errors));
                return _responseHandler.BadRequest<ResetPasswordResponse>(string.Join(", ", errors));
            }

            _logger.LogInformation("Password reset succeeded for User ID: {UserId}. Invalidating old tokens...", user.Id);

            // Invalidate all previous tokens for security
            await _tokenStoreService.InvalidateOldTokensAsync(user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            var response = new ResetPasswordResponse
            {
                UserId = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "USER"
            };

            _logger.LogInformation("ResetPasswordAsync completed successfully for User ID: {UserId}", user.Id);

            return _responseHandler.Success(response, "Password reset successfully. Please log in with your new password.");
        }

        public async Task<Response<bool>> VerifyOtpAsync(VerifyOtpRequest verifyOtpRequest)
        {
            var user = await _userManager.FindByIdAsync(verifyOtpRequest.UserId);
            if (user == null)
                return _responseHandler.NotFound<bool>("User not found.");

            if (user.EmailConfirmed)
                return _responseHandler.Success(true, "Email is already verified.");

            var isOtpValid = await _otpService.ValidateOtpAsync(verifyOtpRequest.UserId, verifyOtpRequest.Otp);
            if (!isOtpValid)
                return _responseHandler.BadRequest<bool>("Invalid or expired OTP.");

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return _responseHandler.BadRequest<bool>("Failed to update user confirmation status.");

            return _responseHandler.Success(true, "Email verified successfully.");
        }

        public async Task<Response<string>> ResendOtpAsync(ResendOtpRequest resendOtpRequest)
        {
            var user = await _userManager.FindByIdAsync(resendOtpRequest.UserId);
            if (user == null)
                return _responseHandler.NotFound<string>("User not found.");

            if (user.EmailConfirmed)
                return _responseHandler.Success<string>(null, "Email is already verified. No need to resend OTP.");

            var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

            await _emailService.SendOtpEmailAsync(user, otp);

            return _responseHandler.Success<string>(null, "OTP resent successfully. Please check your email.");
        }

        public async Task<Response<RefreshTokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Starting RefreshTokenAsync for token: {TokenSnippet}",
                refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));

            try
            {
                var isValid = await _tokenStoreService.IsValidAsync(refreshToken);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid refresh token provided: {TokenSnippet}",
                        refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                    throw new SecurityTokenException("Invalid refresh token");
                }

                // Use repository to find refresh token
                var tokenEntry = await _unitOfWork.Repository<UserRefreshToken>()
                    .FindSingleAsync(r => r.Token == refreshToken);

                if (tokenEntry == null)
                {
                    _logger.LogWarning("No refresh token entry found for token: {TokenSnippet}",
                        refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                    throw new SecurityTokenException("Invalid refresh token");
                }

                var user = await _userManager.FindByIdAsync(tokenEntry.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User not found for refresh token with UserId: {UserId}", tokenEntry.UserId);
                    throw new SecurityTokenException("Invalid user");
                }

                _logger.LogInformation("Invalidating old refresh tokens for user: {UserId}", user.Id);
                await _tokenStoreService.InvalidateOldTokensAsync(user.Id);

                _logger.LogInformation("Generating new access and refresh tokens for user: {UserId}", user.Id);
                var userTokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

                await _tokenStoreService.SaveRefreshTokenAsync(user.Id, userTokens.RefreshToken);
                _logger.LogInformation("New refresh token saved for user: {UserId}", user.Id);

                return _responseHandler.Success(new RefreshTokenResponse
                {
                    AccessToken = userTokens.AccessToken,
                    RefreshToken = userTokens.RefreshToken,
                }, "Token refreshed successfully");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Security token error during refresh token process for token: {TokenSnippet}",
                    refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during refresh token process for token: {TokenSnippet}",
                    refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                throw;
            }
        }

        public async Task<Response<string>> LogoutAsync(ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<string>("User not authenticated");

                await _tokenStoreService.InvalidateOldTokensAsync(userId);
                return _responseHandler.Success<string>(null, "Logged out successfully");
            }
            catch (Exception ex)
            {
                return _responseHandler.ServerError<string>($"An error occurred during logout: {ex.Message}");
            }
        }

        public async Task<Response<string>> ChangePasswordAsync(ClaimsPrincipal userClaims, ChangePasswordRequest request)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<string>("User not authenticated");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return _responseHandler.NotFound<string>("User not found");

                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                    return _responseHandler.BadRequest<string>("Current password is incorrect");

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return _responseHandler.BadRequest<string>(errors);
                }

                await _tokenStoreService.InvalidateOldTokensAsync(userId);
                return _responseHandler.Success<string>(null, "Password changed successfully. Please login again.");
            }
            catch (Exception ex)
            {
                return _responseHandler.ServerError<string>($"An error occurred while changing password: {ex.Message}");
            }
        }

        public async Task<Response<RegisterResponse>> RegisterUserAsync(RegisterRequest registerRequest)
        {
            _logger.LogInformation("RegisterUserAsync started for Email: {Email}", registerRequest.Email);

            // Check if email or phone exists using repository
            var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(registerRequest.Email);
            if (emailExists)
            {
                _logger.LogWarning("Registration failed: Email already exists");
                return _responseHandler.BadRequest<RegisterResponse>("Email is already registered.");
            }

            if (!string.IsNullOrEmpty(registerRequest.PhoneNumber))
            {
                var phoneExists = await _unitOfWork.Users.IsPhoneExistsAsync(registerRequest.PhoneNumber);
                if (phoneExists)
                {
                    _logger.LogWarning("Registration failed: Phone already exists");
                    return _responseHandler.BadRequest<RegisterResponse>("Phone number is already registered.");
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserName = registerRequest.Email,
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber,
                    FirstName = registerRequest.FirstName ?? "",
                    LastName = registerRequest.LastName ?? ""
                };

                var createUserResult = await _userManager.CreateAsync(user, registerRequest.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                    _logger.LogWarning("User creation failed for Email: {Email}. Errors: {Errors}",
                        registerRequest.Email, string.Join(", ", errors));
                    await _unitOfWork.RollbackAsync();
                    return _responseHandler.BadRequest<RegisterResponse>(string.Join(", ", errors));
                }

                // Assign USER role
                await _userManager.AddToRoleAsync(user, "USER");
                _logger.LogInformation("User created and role 'USER' assigned. ID: {UserId}", user.Id);

           

                var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);
                var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

                await _emailService.SendOtpEmailAsync(user, otp);

                await _unitOfWork.CommitAsync();

                var response = new RegisterResponse
                {
                    Email = registerRequest.Email,
                    Id = user.Id,
                    IsEmailConfirmed = false,
                    PhoneNumber = registerRequest.PhoneNumber,
                    Role = "USER",
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                };

                return _responseHandler.Created(response, "User registered successfully. Please verify your email.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error during RegisterUserAsync for Email: {Email}", registerRequest.Email);
                return _responseHandler.BadRequest<RegisterResponse>("An error occurred during registration.");
            }
        }

        public async Task<Response<ServiceProviderApplicationResponse>> RegisterServiceProviderAsync(RegisterServiceProviderRequest request)
        {
            _logger.LogInformation("RegisterServiceProviderAsync started for Email: {Email}", request.Email);

            // ✅ Step 1: Check if email or phone already exists
            if (await _unitOfWork.Users.IsEmailExistsAsync(request.Email))
                return _responseHandler.BadRequest<ServiceProviderApplicationResponse>("Email is already registered.");

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
                await _unitOfWork.Users.IsPhoneExistsAsync(request.PhoneNumber))
                return _responseHandler.BadRequest<ServiceProviderApplicationResponse>("Phone number is already registered.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ✅ Step 2: Create Identity User
                var user = new User
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth
                };

                var createUserResult = await _userManager.CreateAsync(user, request.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                    await _unitOfWork.RollbackAsync();
                    _logger.LogWarning("Failed to create user for Email: {Email} - Errors: {Errors}", request.Email, errors);
                    return _responseHandler.BadRequest<ServiceProviderApplicationResponse>(errors);
                }

                // ✅ Step 3: Assign default role
                await _userManager.AddToRoleAsync(user, "PROVIDER");


                // ✅ Step 4: Handle File Uploads
                string? idDocumentPath = null;
                string? certificatePath = null;
                string? cvPath = null;

                if (request.IdDocument != null)
                {
                    // idDocumentPath = await _fileUploadService.UploadFileAsync(request.IdDocument, "documents");
                }
                    if (request.Certificate != null)
                    { 
                 // certificatePath = await _fileUploadService.UploadFileAsync(request.Certificate, "certificates");
                        }
                 if (request.CVPath != null)
                   {
                     //cvPath = await _fileUploadService.UploadFileAsync(request.CVPath, "cv");
                   }
                if (await _unitOfWork.ServiceProviderApplications.AnyAsync(s => s.NationalId == request.NationalId))
                {
                    await _unitOfWork.RollbackAsync();
                    return _responseHandler.BadRequest<ServiceProviderApplicationResponse>("National ID already exists.");
                }

                // ✅ Step 5: Create Service Provider Application
                var application = new ServiceProviderApplication
                {
                    UserId = user.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    Bio = request.Bio,
                    NationalId = request.NationalId,
                    Experience = request.Experience,
                    HourlyRate = request.HourlyRate,
                    IdDocumentPath = idDocumentPath ?? string.Empty,
                    CertificatePath = certificatePath ?? string.Empty,
                    CVPath = cvPath ?? string.Empty,
                    SelectedCategories = request.SelectedCategoryIds,
                    Status = ServiceProviderApplicationStatus.Pending
                };

                await _unitOfWork.ServiceProviderApplications.AddAsync(application);

                // ✅ Step 6: Send OTP for email verification
                var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);
                await _emailService.SendOtpEmailAsync(user, otp);

                // ✅ Step 7: Commit Transaction
                await _unitOfWork.CommitAsync();

                // ✅ Step 8: Return success response
                var response = new ServiceProviderApplicationResponse
                {
                    ApplicationId = application.Id,
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    Message = "Service provider application submitted successfully. Please verify your email and wait for admin approval.",
                    Status = application.Status,
                    CreatedAt = application.CreatedAt
                };

                _logger.LogInformation("RegisterServiceProviderAsync succeeded for Email: {Email}", request.Email);
                return _responseHandler.Created(response, "Application submitted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error during RegisterServiceProviderAsync for Email: {Email}", request.Email);
                return _responseHandler.BadRequest<ServiceProviderApplicationResponse>("An error occurred during registration.");
            }
        }

        public async Task<Response<RegisterResponse>> CreateAdminAsync(AdminRegisterRequest request)
        {
            _logger.LogInformation("CreateAdminAsync started for Email: {Email}", request.Email);

            // Check if email or phone exists using repository
            var emailExists = await _unitOfWork.Users.IsEmailExistsAsync(request.Email);
            if (emailExists)
                return _responseHandler.BadRequest<RegisterResponse>("Email is already registered.");

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var phoneExists = await _unitOfWork.Users.IsPhoneExistsAsync(request.PhoneNumber);
                if (phoneExists)
                    return _responseHandler.BadRequest<RegisterResponse>("Phone number is already registered.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailConfirmed = true // Auto-confirm admin accounts
                };

                var createUserResult = await _userManager.CreateAsync(user, request.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                    await _unitOfWork.RollbackAsync();
                    return _responseHandler.BadRequest<RegisterResponse>(string.Join(", ", errors));
                }

                // Assign ADMIN role
                await _userManager.AddToRoleAsync(user, "ADMIN");

                await _unitOfWork.CommitAsync();

                var response = new RegisterResponse
                {
                    Email = request.Email,
                    Id = user.Id,
                    IsEmailConfirmed = true,
                    PhoneNumber = request.PhoneNumber,
                    Role = "ADMIN",
                    AccessToken = "",
                    RefreshToken = ""
                };

                return _responseHandler.Created(response, "Admin account created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error during CreateAdminAsync for Email: {Email}", request.Email);
                return _responseHandler.BadRequest<RegisterResponse>("An error occurred during admin creation.");
            }
        }
    }
}
