// 1. Updated DataAccessServiceCollectionExtensions
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
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.DataAccess.Repositories.Implementations;

using ElAnis.Utilities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElAnis.DataAccess.Interfaces;
using ElAnis.DataAccess.Repositories;
using ElAnis.DataAccess.Services.ServiceProvider;
using ElAnis.DataAccess.Services.ServicePricing;
using ElAnis.DataAccess.Services.ServiceRequest;
using ElAnis.DataAccess.Services.Payment;


namespace ElAnis.DataAccess.Extensions
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register Specific Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IServiceProviderApplicationRepository, ServiceProviderApplicationRepository>();
            services.AddScoped<IServiceProviderProfileRepository, ServiceProviderProfileRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceProviderCategoryRepository, ServiceProviderCategoryRepository>();
            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMemoryCache();

            // Core Services
            services.AddScoped<IOTPService, OTPServiceInMemory>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageUploadService, CloudinaryImageUploadService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Business Logic Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthGoogleService, AuthGoogleService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IServiceProviderService, ServiceProviderService>();
            services.AddScoped<IServicePricingService, ServicePricingService>();
            services.AddScoped<IServiceRequestService, ServiceRequestService>();
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

        public static IServiceCollection AddAllDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDatabase(configuration)
                .AddRepositories()
                .AddUnitOfWork()
                .AddApplicationServices()
                .AddEmailServices(configuration);
        }
    }
}