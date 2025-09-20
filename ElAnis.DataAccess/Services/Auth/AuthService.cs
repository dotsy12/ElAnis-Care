using System.Security.Claims;

using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Services.Email;
using ElAnis.DataAccess.Services.OTP;
using ElAnis.DataAccess.Services.Token;
using ElAnis.Entities.DTO.Account.Auth;
using ElAnis.Entities.DTO.Account.Auth.Login;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Account.Auth.ResetPassword;
using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using LoginRequest = ElAnis.Entities.DTO.Account.Auth.Login.LoginRequest;
using ResetPasswordRequest = ElAnis.Entities.DTO.Account.Auth.ResetPassword.ResetPasswordRequest;


namespace ElAnis.DataAccess.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthContext _context;
        private readonly IEmailService _emailService;
        private readonly IOTPService _otpService;
        private readonly ResponseHandler _responseHandler;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<User> userManager, AuthContext context, IEmailService emailService, IOTPService otpService, ResponseHandler responseHandler, ITokenStoreService tokenStoreService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _otpService = otpService;
            _responseHandler = responseHandler;
            _tokenStoreService = tokenStoreService;
            _logger = logger;
        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            // Find user by email or phone number
            User? user = await FindUserByEmailOrPhoneAsync(loginRequest.Email, loginRequest.PhoneNumber);

            if (user == null)
                return _responseHandler.NotFound<LoginResponse>("User not found.");

            // Check password
            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return _responseHandler.BadRequest<LoginResponse>("Invalid password.");

            // Check if email is confirmed
            if (!user.EmailConfirmed)
                return _responseHandler.BadRequest<LoginResponse>("Email is not verified. Please verify your email first.");

            //// If OTP is not provided, generate and send OTP
            //if (string.IsNullOrEmpty(loginRequest.Otp))
            //{
            //    var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);
            //    await _emailService.SendOtpEmailAsync(user, otp);
            //    return _responseHandler.Success<LoginResponse>(null, "OTP sent to your email. Please provide the OTP to complete login.");
            //}

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);


            // Verify OTP
            //var isOtpValid = await _otpService.ValidateOtpAsync(user.Id, loginRequest.Otp);
            //if (!isOtpValid)
            //    return _responseHandler.BadRequest<LoginResponse>("Invalid or expired OTP.");

            // Generate tokens
            var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);

            var response = new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault(),
                IsEmailConfirmed = user.EmailConfirmed,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
            };

            return _responseHandler.Success(response, "Login successful.");
        }
      
        public async Task<Response<ForgetPasswordResponse>> ForgotPasswordAsync(ForgetPasswordRequest model)
        {
            _logger.LogInformation("Starting ForgotPasswordAsync for Email: {Email}, Phone: {Phone}", model.Email, model.PhoneNumber);

            // Find user by email or phone number
            User? user = await FindUserByEmailOrPhoneAsync(model.Email, model.PhoneNumber);

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
                //return _responseHandler.InternalServerError<ForgetPasswordResponse>("Failed to send OTP.");
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

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Starting RefreshTokenAsync for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));

            try
            {
                var isValid = await _tokenStoreService.IsValidAsync(refreshToken);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid refresh token provided: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                    throw new SecurityTokenException("Invalid refresh token");
                }

                var tokenEntry = await _context.UserRefreshTokens
                    .FirstOrDefaultAsync(r => r.Token == refreshToken);
                if (tokenEntry == null)
                {
                    _logger.LogWarning("No refresh token entry found for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
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

                return new RefreshTokenResponse
                {
                    AccessToken = userTokens.AccessToken,
                    RefreshToken = userTokens.RefreshToken,
                };
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Security token error during refresh token process for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during refresh token process for token: {TokenSnippet}", refreshToken.Substring(0, Math.Min(8, refreshToken.Length)));
                throw;
            }
        }

        // Helpers methods 
        private async Task<string?> CheckIfEmailOrPhoneExists(string email, string? phoneNumber)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
                return "Email is already registered.";
            if (!string.IsNullOrEmpty(phoneNumber) && await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber))
                return "Phone number is already registered.";
            return null;
        }
        private async Task<User?> FindUserByEmailOrPhoneAsync(string email, string phone)
        {
            if (!string.IsNullOrEmpty(email))
                return await _userManager.FindByEmailAsync(email);
            if (!string.IsNullOrEmpty(phone))
                return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            return null;
        }

        public async Task<Response<string>> LogoutAsync(ClaimsPrincipal userClaims)
        {
            try
            {
                // Get user ID from claims
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return _responseHandler.Unauthorized<string>("User not authenticated");
                }

                // Invalidate all refresh tokens for this user
                await _tokenStoreService.InvalidateOldTokensAsync(userId);

                return _responseHandler.Success<string>(null,"Logged out successfully");
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
                // Get user ID from claims
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return _responseHandler.Unauthorized<string>("User not authenticated");
                }

                // Find user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return _responseHandler.NotFound<string>("User not found");
                }

                // Verify current password
                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return _responseHandler.BadRequest<string>("Current password is incorrect");
                }

                // Change password
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return _responseHandler.BadRequest<string>(errors);
                }

                // Invalidate all existing refresh tokens for security
                await _tokenStoreService.InvalidateOldTokensAsync(userId);

                return _responseHandler.Success<string>(null,"Password changed successfully. Please login again.");
            }
            catch (Exception ex)
            {
                return _responseHandler.ServerError<string>($"An error occurred while changing password: {ex.Message}");
            }
        }

		public async Task<Response<RegisterResponse>> RegisterUserAsync(RegisterRequest registerRequest)
		{
			_logger.LogInformation("RegisterUserAsync started for Email: {Email}", registerRequest.Email);

			var emailPhoneCheck = await CheckIfEmailOrPhoneExists(registerRequest.Email, registerRequest.PhoneNumber);
			if (emailPhoneCheck != null)
			{
				_logger.LogWarning("Registration failed: {Reason}", emailPhoneCheck);
				return _responseHandler.BadRequest<RegisterResponse>(emailPhoneCheck);
			}

			using var transaction = await _context.Database.BeginTransactionAsync();
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
					return _responseHandler.BadRequest<RegisterResponse>(string.Join(", ", errors));
				}

				// Assign USER role
				await _userManager.AddToRoleAsync(user, "USER");
				_logger.LogInformation("User created and role 'USER' assigned. ID: {UserId}", user.Id);

				// Create user preferences
				var preferences = new UserPreferences { UserId = user.Id };
				_context.UserPreferences.Add(preferences);

				var tokens = await _tokenStoreService.GenerateAndStoreTokensAsync(user.Id, user);
				var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);

				await _emailService.SendOtpEmailAsync(user, otp);

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

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
				await transaction.RollbackAsync();
				_logger.LogError(ex, "Error during RegisterUserAsync for Email: {Email}", registerRequest.Email);
				return _responseHandler.BadRequest<RegisterResponse>("An error occurred during registration.");
			}
		}


		public async Task<Response<ServiceProviderApplicationResponse>> RegisterServiceProviderAsync(RegisterServiceProviderRequest request)
		{
			_logger.LogInformation("RegisterServiceProviderAsync started for Email: {Email}", request.Email);

			var emailPhoneCheck = await CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber);
			if (emailPhoneCheck != null)
			{
				return _responseHandler.BadRequest<ServiceProviderApplicationResponse>(emailPhoneCheck);
			}

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// Create user account with USER role first (will be upgraded to SERVICE_PROVIDER after approval)
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
					var errors = createUserResult.Errors.Select(e => e.Description).ToList();
					return _responseHandler.BadRequest<ServiceProviderApplicationResponse>(string.Join(", ", errors));
				}

				// Assign USER role temporarily
				await _userManager.AddToRoleAsync(user, "USER");

				// Handle file uploads (you'll need to implement file upload service)
				string? idDocumentPath = null;
				string? certificatePath = null;

				if (request.IdDocument != null)
				{
					// idDocumentPath = await _fileUploadService.UploadFileAsync(request.IdDocument, "documents");
				}

				if (request.Certificate != null)
				{
					// certificatePath = await _fileUploadService.UploadFileAsync(request.Certificate, "certificates");
				}

				// Create service provider application
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
					IdDocumentPath = idDocumentPath ?? "",
					CertificatePath = certificatePath ?? "",
					SelectedCategories = request.SelectedCategoryIds,
					Status = ServiceProviderApplicationStatus.Pending
				};

				_context.ServiceProviderApplications.Add(application);

				// Send notification OTP
				var otp = await _otpService.GenerateAndStoreOtpAsync(user.Id);
				await _emailService.SendOtpEmailAsync(user, otp);

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				var response = new ServiceProviderApplicationResponse
				{
					ApplicationId = application.Id,
					UserId = user.Id,
					Email = user.Email ?? "",
					Message = "Service provider application submitted successfully. Please verify your email and wait for admin approval.",
					Status = application.Status,
					CreatedAt = application.CreatedAt
				};

				return _responseHandler.Created(response, "Application submitted successfully.");
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				_logger.LogError(ex, "Error during RegisterServiceProviderAsync for Email: {Email}", request.Email);
				return _responseHandler.BadRequest<ServiceProviderApplicationResponse>("An error occurred during registration.");
			}
		}

		public async Task<Response<RegisterResponse>> CreateAdminAsync(AdminRegisterRequest request)
		{
			_logger.LogInformation("CreateAdminAsync started for Email: {Email}", request.Email);

			var emailPhoneCheck = await CheckIfEmailOrPhoneExists(request.Email, request.PhoneNumber);
			if (emailPhoneCheck != null)
			{
				return _responseHandler.BadRequest<RegisterResponse>(emailPhoneCheck);
			}

			using var transaction = await _context.Database.BeginTransactionAsync();
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
					return _responseHandler.BadRequest<RegisterResponse>(string.Join(", ", errors));
				}

				// Assign ADMIN role
				await _userManager.AddToRoleAsync(user, "ADMIN");

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

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
				await transaction.RollbackAsync();
				_logger.LogError(ex, "Error during CreateAdminAsync for Email: {Email}", request.Email);
				return _responseHandler.BadRequest<RegisterResponse>("An error occurred during admin creation.");
			}
		}
	}
}
