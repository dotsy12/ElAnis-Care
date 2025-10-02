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

            // Add roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add ServiceProvider specific claims if user is a service provider
            if (roles.Contains("SERVICE_PROVIDER") || roles.Contains("PROVIDER"))
            {
                var serviceProvider = await _unitOfWork.ServiceProviderProfiles
                    .GetByUserIdAsync(appUser.Id);

                if (serviceProvider != null)
                {
                    claims.Add(new Claim("ServiceProviderId", serviceProvider.Id.ToString()));
                    claims.Add(new Claim("ServiceProviderStatus", serviceProvider.Status.ToString()));
                    claims.Add(new Claim("IsAvailable", serviceProvider.IsAvailable.ToString()));
                }
            }

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
            return tokenHandler.WriteToken(token);
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
