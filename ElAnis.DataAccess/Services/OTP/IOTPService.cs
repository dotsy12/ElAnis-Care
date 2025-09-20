namespace ElAnis.DataAccess.Services.OTP
{
    public interface IOTPService
    {
        Task<string> GenerateAndStoreOtpAsync(string userId);
        Task<bool> ValidateOtpAsync(string userId, string otp);
    }
}
