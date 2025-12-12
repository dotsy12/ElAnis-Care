using ElAnis.API.Extensions;
using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Extensions;
using ElAnis.DataAccess.Hubs;
using ElAnis.DataAccess.Seeder;
using ElAnis.DataAccess.Services.Payment;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Configurations;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text;

// 👇 حل مشكلة الـ Ambiguous Reference
using IdentityRole = ElAnis.Entities.Models.Auth.Identity.Role;

namespace ElAnisPlatform
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // تحميل ملفات الـ appsettings حسب الـ Environment
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            builder.Services.AddControllers();
            builder.Host.UseSerilogLogging();

            // تسجيل كل الـ Data Access Services في سطر واحد
            builder.Services.AddAllDataAccessServices(builder.Configuration);

            // Configure API Behavior
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(
                options => options.SuppressModelStateInvalidFilter = true
            );

            // IOptions Pattern
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
           
            builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authorization:Google"));

            // Stripe Configuration
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });
            // Payment Service
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddScoped<ResponseHandler>();
            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
            builder.Services.AddFluentValidation();
            builder.Services.AddResendOtpRateLimiter();

            // تفعيل CORS
            builder.Services.AddCorsPolicy(builder.Configuration);

            builder.Services.AddDataProtection()
                .PersistKeysToDbContext<AuthContext>()
                .SetApplicationName("AuthStarter");

            // Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
                configuration.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configuration);
            });

            builder.Services.AddSwagger();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // 🔹 Seed Roles & Users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();

                // ✅ هنا نستخدم الـ IdentityRole
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await RoleSeeder.SeedAsync(roleManager);
                await UserSeeder.SeedAsync(userManager);
            }

            // 🔹 Swagger always enabled
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElAnis API V1");
                c.RoutePrefix = string.Empty; // Swagger يفتح مباشرة
            });

            app.UseHttpsRedirection();

            // 🔹 ترتيب الميدل وير
            app.UseCors("AllowAll"); // لازم يكون هنا قبل Authentication

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            // SignalR Hub Mapping
            app.MapHub<ChatHub>("/chatHub");
            app.Run();
        }
    }
}
