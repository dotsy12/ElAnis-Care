using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using ElAnis.DataAccess.ApplicationContext;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models.Auth.UserTokens;
using ElAnis.Utilities.Configurations;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ElAnis.DataAccess.Services.Token
{
    public class TokenStoreService : ITokenStoreService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public TokenStoreService(
            IOptions<JwtSettings> jwtOptions,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _userManager = userManager;
            _unitOfWork = unitOfWork;

            if (string.IsNullOrEmpty(_jwtSettings.SigningKey))
            {
                throw new ArgumentException("JWT SigningKey is not configured.");
            }
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        }

        public async Task<string> CreateAccessTokenAsync(User appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Email, appUser.Email ?? ""),
                new Claim(ClaimTypes.GivenName, appUser.UserName ?? ""),
                new Claim("UserId", appUser.Id.ToString()),
                new Claim("FullName", $"{appUser.FirstName} {appUser.LastName}".Trim())
            };

            // ✅ إضافة الأدوار
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // ✅ التعامل مع البروفايدر
            if (roles.Contains("Provider") || roles.Contains("PROVIDER"))
            {
                // 🔍 أولاً: نحاول نجيب الـ Profile (لو موجود ومتوافق عليه)
                var serviceProvider = await _unitOfWork.ServiceProviderProfiles
                    .GetByUserIdAsync(appUser.Id);

                if (serviceProvider != null)
                {
                    // ✅ البروفايدر عنده Profile (معناها اتوافق عليه)
                    claims.Add(new Claim("ServiceProviderId", serviceProvider.Id.ToString()));
                    claims.Add(new Claim("ServiceProviderStatus", serviceProvider.Status.ToString()));
                    claims.Add(new Claim("IsAvailable", serviceProvider.IsAvailable.ToString()));

                    Console.WriteLine($"✅ [Token] Provider Profile Found - Status: {serviceProvider.Status}");
                }
                else
                {
                    // 🟡 البروفايدر لسه مقدم طلب (مفيش Profile)
                    var application = await _unitOfWork.ServiceProviderApplications
                        .FindSingleAsync(a => a.UserId == appUser.Id);

                    if (application != null)
                    {
                        claims.Add(new Claim("ApplicationId", application.Id.ToString()));
                        claims.Add(new Claim("ServiceProviderStatus", application.Status.ToString()));

                        Console.WriteLine($"🟡 [Token] Application Found - Status: {application.Status}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ [Token] No Profile or Application found for user {appUser.Id}");
                    }
                }
            }

            // ✅ إنشاء الـ Token
            var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            // 🔍 للـ Debugging: اطبع الـ Claims اللي في التوكن
            Console.WriteLine($"🎫 [Token Generated] User: {appUser.Email}, Roles: {string.Join(", ", roles)}");
            var statusClaim = claims.FirstOrDefault(c => c.Type == "ServiceProviderStatus");
            if (statusClaim != null)
            {
                Console.WriteLine($"   └─ ServiceProviderStatus: {statusClaim.Value}");
            }

            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            var userRefreshTokenRepo = _unitOfWork.Repository<UserRefreshToken>();

            await userRefreshTokenRepo.AddAsync(new UserRefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiryDateUtc = DateTime.UtcNow.AddDays(7),
                IsUsed = false
            });

            await _unitOfWork.CompleteAsync();
        }

        public async Task InvalidateOldTokensAsync(string userId)
        {
            var userRefreshTokenRepo = _unitOfWork.Repository<UserRefreshToken>();

            var tokens = await userRefreshTokenRepo.FindAsync(r => r.UserId == userId);

            if (tokens.Any())
            {
                userRefreshTokenRepo.DeleteRange(tokens);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> IsValidAsync(string refreshToken)
        {
            var userRefreshTokenRepo = _unitOfWork.Repository<UserRefreshToken>();

            return await userRefreshTokenRepo.AnyAsync(r =>
                r.Token == refreshToken &&
                !r.IsUsed &&
                r.ExpiryDateUtc > DateTime.UtcNow);
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateAndStoreTokensAsync(string userId, User user)
        {
            var accessToken = await CreateAccessTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(userId, refreshToken);
            return (accessToken, refreshToken);
        }
    }
}