
using ElAnis.API.Extensions;
using ElAnis.DataAccess.ApplicationContext;
using ElAnis.DataAccess.Extensions;
using ElAnis.DataAccess.Seeder;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Configurations;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text;

namespace ElAnisPlatform
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Host.UseSerilogLogging();

			// DbContext
			builder.Services.AddDbContext<AuthContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// Configure API Behavior
			builder.Services.AddControllers().ConfigureApiBehaviorOptions(
				options => options.SuppressModelStateInvalidFilter = true
			);

			// IOptions Pattern
			builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
			builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
			builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
			builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Authorization:Google"));

			builder.Services.AddApplicationServices();
			builder.Services.AddScoped<ResponseHandler>();
			builder.Services.AddDatabase(builder.Configuration);
			builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
			builder.Services.AddEmailServices(builder.Configuration);
			builder.Services.AddFluentValidation();
			builder.Services.AddResendOtpRateLimiter();

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
				var roleManager = services.GetRequiredService<RoleManager<ElAnis.Entities.Models.Auth.Identity.Role>>();

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
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.Run();

		}
	}
}
