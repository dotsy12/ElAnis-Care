using ElAnis.DataAccess.Services.Auth;
using ElAnis.DataAccess.Services.OAuth;
using ElAnis.Entities.DTO.Account.Auth;
using ElAnis.Entities.DTO.Account.Auth.Login;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Account.Auth.ResetPassword;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace ElAnis.API.Controllers
{
	/// <summary>
	/// Controller for user authentication and account management operations
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly ResponseHandler _responseHandler;
		private readonly IValidator<RegisterRequest> _registerValidator;
		private readonly IValidator<RegisterServiceProviderRequest> _serviceProviderRegisterValidator;
		private readonly IValidator<AdminRegisterRequest> _adminRegisterValidator;
		private readonly IValidator<LoginRequest> _loginValidator;
		private readonly IValidator<ForgetPasswordRequest> _forgetPasswordValidator;
		private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
		private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
		private readonly IAuthGoogleService _authGoogleService;

		/// <summary>
		/// Initializes a new instance of AccountController
		/// </summary>
		public AccountController(
			IAuthService authService,
			ResponseHandler responseHandler,
			IValidator<RegisterRequest> registerValidator,
			IValidator<RegisterServiceProviderRequest> serviceProviderRegisterValidator,
			IValidator<AdminRegisterRequest> adminRegisterValidator,
			IValidator<LoginRequest> loginValidator,
			IValidator<ForgetPasswordRequest> forgetPasswordValidator,
			IValidator<ResetPasswordRequest> resetPasswordValidator,
			IAuthGoogleService authGoogleService,
			IValidator<ChangePasswordRequest> changePasswordValidator)
		{
			_authService = authService;
			_responseHandler = responseHandler;
			_registerValidator = registerValidator;
			_serviceProviderRegisterValidator = serviceProviderRegisterValidator;
			_adminRegisterValidator = adminRegisterValidator;
			_loginValidator = loginValidator;
			_forgetPasswordValidator = forgetPasswordValidator;
			_resetPasswordValidator = resetPasswordValidator;
			_authGoogleService = authGoogleService;
			_changePasswordValidator = changePasswordValidator;
		}

		/// <summary>
		/// Authenticates a user with email and password
		/// </summary>
		/// <param name="request">Login credentials including email and password</param>
		/// <returns>JWT token and user information upon successful authentication</returns>
		/// <response code="200">Login successful, returns JWT token and user data</response>
		/// <response code="400">Invalid request data or validation errors</response>
		/// <response code="401">Invalid credentials or account not verified</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("login")]
		[ProducesResponseType(typeof(Response<LoginResponse>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<LoginResponse>>> Login([FromBody] LoginRequest request)
		{
			ValidationResult validationResult = await _loginValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
					_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.LoginAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Authenticates a user using Google OAuth
		/// </summary>
		/// <param name="googleLoginDto">Google ID token for authentication</param>
		/// <returns>JWT token and user information upon successful Google authentication</returns>
		/// <response code="200">Google login successful, returns JWT token</response>
		/// <response code="400">Invalid Google token or request data</response>
		/// <response code="401">Google authentication failed</response>
		/// <response code="500">Internal server error or user creation failed</response>
		[HttpPost("login/google")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest googleLoginDto)
		{
			if (!ModelState.IsValid)
				return _responseHandler.HandleModelStateErrors(ModelState);

			try
			{
				var token = await _authGoogleService.AuthenticateWithGoogleAsync(googleLoginDto.IdToken);
				var response = _responseHandler.Success(token, "Logged in with Google successfully");
				return StatusCode((int)response.StatusCode, response);
			}
			catch (UnauthorizedAccessException ex)
			{
				var response = _responseHandler.Unauthorized<string>("Google authentication failed: " + ex.Message);
				return StatusCode((int)response.StatusCode, response);
			}
			catch (UserCreationException ex)
			{
				var response = _responseHandler.InternalServerError<string>("User creation failed: " + ex.Message);
				return StatusCode((int)response.StatusCode, response);
			}
			catch (Exception ex)
			{
				var response = _responseHandler.ServerError<string>("An error occurred: " + ex.Message);
				return StatusCode((int)response.StatusCode, response);
			}
		}

		/// <summary>
		/// Registers a new regular user account
		/// </summary>
		/// <param name="request">User registration data including personal information and credentials</param>
		/// <returns>Registration confirmation and OTP verification instructions</returns>
		/// <response code="201">User registered successfully, OTP sent for verification</response>
		/// <response code="400">Invalid request data, validation errors, or email already exists</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("register-user")]
		[ProducesResponseType(typeof(Response<RegisterResponse>), 201)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<RegisterResponse>>> RegisterUser([FromForm] RegisterRequest request)
		{
			ValidationResult validationResult = await _registerValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
					_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.RegisterUserAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Registers a new service provider application
		/// </summary>
		/// <param name="request">Service provider registration data including personal info, documents, and categories</param>
		/// <returns>Application submission confirmation</returns>
		/// <response code="201">Service provider application submitted successfully</response>
		/// <response code="400">Invalid request data, validation errors, or missing required documents</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("register-service-provider")]
		[ProducesResponseType(typeof(Response<ServiceProviderApplicationResponse>), 201)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<ServiceProviderApplicationResponse>>> RegisterServiceProvider([FromForm] RegisterServiceProviderRequest request)
		{
			ValidationResult validationResult = await _serviceProviderRegisterValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
					_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.RegisterServiceProviderAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Creates a new admin account (Admin only)
		/// </summary>
		/// <param name="request">Admin account creation data</param>
		/// <returns>Admin account creation confirmation</returns>
		/// <response code="201">Admin account created successfully</response>
		/// <response code="400">Invalid request data or validation errors</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="403">Forbidden - Admin access required</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("create-admin")]
		[Authorize(Policy = "AdminOnly")]
		[ProducesResponseType(typeof(Response<RegisterResponse>), 201)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 403)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<RegisterResponse>>> CreateAdmin([FromBody] AdminRegisterRequest request)
		{
			ValidationResult validationResult = await _adminRegisterValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
					_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.CreateAdminAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Verifies OTP code sent to user's email
		/// </summary>
		/// <param name="model">OTP verification data including email and OTP code</param>
		/// <returns>Verification result and account activation status</returns>
		/// <response code="200">OTP verified successfully, account activated</response>
		/// <response code="400">Invalid OTP code, expired OTP, or invalid request data</response>
		/// <response code="404">User not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("verify-otp")]
		[ProducesResponseType(typeof(Response<bool>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<bool>>> VerifyOtp([FromBody] VerifyOtpRequest model)
		{
			if (!ModelState.IsValid)
				return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
					_responseHandler.BadRequest<object>("Invalid input data."));

			var result = await _authService.VerifyOtpAsync(model);
			return StatusCode((int)result.StatusCode, result);
		}

		/// <summary>
		/// Resends OTP verification code to user's email (Rate Limited)
		/// </summary>
		/// <param name="model">Email address to resend OTP to</param>
		/// <returns>OTP resend confirmation</returns>
		/// <response code="200">OTP resent successfully</response>
		/// <response code="400">Invalid request data or user already verified</response>
		/// <response code="404">User not found</response>
		/// <response code="429">Too many requests - rate limit exceeded</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("resend-otp")]
		[EnableRateLimiting("SendOtpPolicy")]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 429)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<string>>> ResendOtp([FromBody] ResendOtpRequest model)
		{
			if (!ModelState.IsValid)
				return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
					_responseHandler.BadRequest<object>("Invalid input data."));

			var result = await _authService.ResendOtpAsync(model);
			return StatusCode((int)result.StatusCode, result);
		}

		/// <summary>
		/// Initiates password reset process by sending reset token to user's email
		/// </summary>
		/// <param name="request">Email address for password reset</param>
		/// <returns>Password reset initiation confirmation</returns>
		/// <response code="200">Password reset email sent successfully</response>
		/// <response code="400">Invalid email format or validation errors</response>
		/// <response code="404">User not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("forget-password")]
		[ProducesResponseType(typeof(Response<ForgetPasswordResponse>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<ForgetPasswordResponse>>> ForgetPassword([FromBody] ForgetPasswordRequest request)
		{
			ValidationResult validationResult = await _forgetPasswordValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
					_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.ForgotPasswordAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Resets user password using reset token
		/// </summary>
		/// <param name="request">Password reset data including token and new password</param>
		/// <returns>Password reset confirmation</returns>
		/// <response code="200">Password reset successfully</response>
		/// <response code="400">Invalid or expired token, validation errors, or weak password</response>
		/// <response code="404">User not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("reset-password")]
		[ProducesResponseType(typeof(Response<ResetPasswordResponse>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 404)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<ActionResult<Response<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			ValidationResult validationResult = await _resetPasswordValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
					_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.ResetPasswordAsync(request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Refreshes JWT token using refresh token
		/// </summary>
		/// <param name="refreshToken">Valid refresh token</param>
		/// <returns>New JWT token and refresh token</returns>
		/// <response code="200">Token refreshed successfully</response>
		/// <response code="400">Invalid or missing refresh token</response>
		/// <response code="401">Expired or invalid refresh token</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("refresh-token")]
		[ProducesResponseType(typeof(Response<RefreshTokenResponse>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
				return BadRequest(_responseHandler.BadRequest<string>("RefreshTokenIsNotFound"));
			try
			{
				var newTokens = await _authService.RefreshTokenAsync(refreshToken);
				return Ok(_responseHandler.Success<RefreshTokenResponse>(newTokens.Data, "User token refreshed successfully"));
			}
			catch (SecurityTokenException ex)
			{
				return Unauthorized(_responseHandler.Unauthorized<string>(ex.Message));
			}
			catch (Exception ex)
			{
				var error = ex.InnerException?.Message ?? ex.Message;
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					_responseHandler.BadRequest<string>("UnexpectedError" + ": " + error)
				);
			}
		}

		/// <summary>
		/// Changes user password (requires authentication)
		/// </summary>
		/// <param name="request">Current password and new password</param>
		/// <returns>Password change confirmation</returns>
		/// <response code="200">Password changed successfully</response>
		/// <response code="400">Invalid current password, validation errors, or weak new password</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("change-password")]
		[Authorize]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 400)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			var validationResult = await _changePasswordValidator.ValidateAsync(request);
			if (!validationResult.IsValid)
			{
				var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
				return BadRequest(_responseHandler.BadRequest<object>(errors));
			}

			var response = await _authService.ChangePasswordAsync(User, request);
			return StatusCode((int)response.StatusCode, response);
		}

		/// <summary>
		/// Logs out user and invalidates their token (requires authentication)
		/// </summary>
		/// <returns>Logout confirmation</returns>
		/// <response code="200">User logged out successfully</response>
		/// <response code="401">Unauthorized - Authentication required</response>
		/// <response code="500">Internal server error</response>
		[HttpPost("logout")]
		[Authorize]
		[ProducesResponseType(typeof(Response<string>), 200)]
		[ProducesResponseType(typeof(Response<object>), 401)]
		[ProducesResponseType(typeof(Response<object>), 500)]
		public async Task<IActionResult> Logout()
		{
			var response = await _authService.LogoutAsync(User);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}