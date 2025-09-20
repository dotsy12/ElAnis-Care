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

		[HttpPost("login")]
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

		[HttpPost("login/google")]
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

		[HttpPost("register-user")]
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

		[HttpPost("register-service-provider")]
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

		[HttpPost("create-admin")]
		[Authorize(Policy = "AdminOnly")]
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

		[HttpPost("verify-otp")]
		public async Task<ActionResult<Response<bool>>> VerifyOtp([FromBody] VerifyOtpRequest model)
		{
			if (!ModelState.IsValid)
				return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
					_responseHandler.BadRequest<object>("Invalid input data."));

			var result = await _authService.VerifyOtpAsync(model);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpPost("resend-otp")]
		[EnableRateLimiting("SendOtpPolicy")]
		public async Task<ActionResult<Response<string>>> ResendOtp([FromBody] ResendOtpRequest model)
		{
			if (!ModelState.IsValid)
				return StatusCode((int)_responseHandler.BadRequest<object>("Invalid input data.").StatusCode,
					_responseHandler.BadRequest<object>("Invalid input data."));

			var result = await _authService.ResendOtpAsync(model);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpPost("forget-password")]
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

		[HttpPost("reset-password")]
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

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
				return BadRequest(_responseHandler.BadRequest<string>("RefreshTokenIsNotFound"));
			try
			{
				var newTokens = await _authService.RefreshTokenAsync(refreshToken);
				return Ok(_responseHandler.Success<RefreshTokenResponse>(newTokens, "User token refreshed successfully"));
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

		[HttpPost("change-password")]
		[Authorize]
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

		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			var response = await _authService.LogoutAsync(User);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}