using System.Security.Claims;

using ElAnis.Entities.DTO.Account.Auth;
using ElAnis.Entities.DTO.Account.Auth.Login;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Account.Auth.ResetPassword;
using ElAnis.Entities.Shared.Bases;

using LoginRequest = ElAnis.Entities.DTO.Account.Auth.Login.LoginRequest;
using ResetPasswordRequest = ElAnis.Entities.DTO.Account.Auth.ResetPassword.ResetPasswordRequest;


namespace ElAnis.DataAccess.Services.Auth
{
    public interface IAuthService
    {
        Task<Response<LoginResponse>> LoginAsync(LoginRequest loginRequest);
        Task<Response<ForgetPasswordResponse>> ForgotPasswordAsync(ForgetPasswordRequest model);
        Task<Response<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest model);
        Task<Response<bool>> VerifyOtpAsync(VerifyOtpRequest verifyOtpRequest);
        Task<Response<string>> ResendOtpAsync(ResendOtpRequest resendOtpRequest);
        Task<Response<RefreshTokenResponse>> RefreshTokenAsync(string refreshToken);
        Task<Response<string>> LogoutAsync(ClaimsPrincipal userClaims);
        Task<Response<string>> ChangePasswordAsync(ClaimsPrincipal userClaims, ChangePasswordRequest request);
        Task<Response<RegisterResponse>> RegisterUserAsync(RegisterRequest registerRequest);
        Task<Response<ServiceProviderApplicationResponse>> RegisterServiceProviderAsync(RegisterServiceProviderRequest request);
        Task<Response<RegisterResponse>> CreateAdminAsync(AdminRegisterRequest request);


    }
}
