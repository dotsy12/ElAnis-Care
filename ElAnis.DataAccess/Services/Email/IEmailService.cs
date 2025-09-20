using ElAnis.Entities.Models.Auth.Identity;

namespace ElAnis.DataAccess.Services.Email
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(User applicationUser, string otp);
    }
}
