using System.Net;
using System.Net.Mail;

using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Services.Admin;
using ElAnis.DataAccess.Services.Auth;
using ElAnis.DataAccess.Services.Category;
using ElAnis.DataAccess.Services.Email;
using ElAnis.DataAccess.Services.ImageUploading;
using ElAnis.DataAccess.Services.OAuth;
using ElAnis.DataAccess.Services.OTP;
using ElAnis.DataAccess.Services.Token;
using ElAnis.Utilities.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElAnis.DataAccess.Extensions
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DevCS")));

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
			services.AddMemoryCache();
			//services.AddScoped<IOTPService, OTPServiceRedis>();
			services.AddScoped<IOTPService, OTPServiceInMemory>();
			services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthGoogleService, AuthGoogleService>();
			services.AddScoped<IAdminService, AdminService>();
			services.AddScoped<ICategoryService, CategoryService>();

			return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

            services.AddFluentEmail(emailSettings.FromEmail)
                .AddSmtpSender(new SmtpClient(emailSettings.SmtpServer)
                {
                    Port = emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                    EnableSsl = emailSettings.EnableSsl
                });

            return services;
        }
    }
}
