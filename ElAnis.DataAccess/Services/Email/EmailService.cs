using ElAnis.Entities.Models.Auth.Identity;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ElAnis.DataAccess.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IFluentEmail fluentEmail, UserManager<User> userManager, ILogger<EmailService> logger)
        {
            _fluentEmail = fluentEmail;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SendOtpEmailAsync(User user, string otp)
        {
            try
            {
                var rootPath = Directory.GetCurrentDirectory();
                var templatePath = Path.Combine(rootPath, "wwwroot", "EmailTemplates", "OtpVerificationEmail.html");

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"OTP Email Template not found at path: {templatePath}");
                    throw new FileNotFoundException("OTP Email Template not found.", templatePath);
                }

                var emailTemplate = await File.ReadAllTextAsync(templatePath);

                emailTemplate = emailTemplate
                    .Replace("{OtpCode}", otp)
                    .Replace("{CurrentYear}", DateTime.UtcNow.Year.ToString())
                    .Replace("{Username}", user.UserName ?? user.Email ?? "User");

                var sendResult = await _fluentEmail
                    .To(user.Email)
                    .Subject("Email Confirmation Code")
                    .Body(emailTemplate, isHtml: true)
                    .SendAsync();

                if (!sendResult.Successful)
                {
                    _logger.LogError($"Failed to send OTP email to {user.Email}. Errors: {string.Join(", ", sendResult.ErrorMessages)}");
                    throw new Exception("Failed to send OTP email.");
                }

                _logger.LogInformation($"OTP email successfully sent to {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending OTP email to {user.Email}");
                throw;
            }
        }
    }
}
