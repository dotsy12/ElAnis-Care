using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ElAnis.DataAccess.Services.OTP
{
	public class OTPServiceInMemory : IOTPService
	{
		private readonly IMemoryCache _cache;
		private readonly ILogger<OTPServiceInMemory> _logger;

		public OTPServiceInMemory(IMemoryCache cache, ILogger<OTPServiceInMemory> logger)
		{
			_cache = cache;
			_logger = logger;
		}

		public async Task<string> GenerateAndStoreOtpAsync(string userId)
		{
			var otp = GenerateOtp();

			// تخزين OTP في الكاش لمدة 5 دقائق
			_cache.Set($"otp:{userId}", otp, TimeSpan.FromMinutes(5));

			_logger.LogInformation("OTP generated and stored for UserId: {UserId}. Expiry: 5 Minutes", userId);

			return await Task.FromResult(otp);
		}

		public async Task<bool> ValidateOtpAsync(string userId, string otp)
		{
			if (_cache.TryGetValue($"otp:{userId}", out string? storedOtp))
			{
				if (storedOtp == otp)
				{
					_cache.Remove($"otp:{userId}");
					_logger.LogInformation("OTP validated successfully for UserId: {UserId}", userId);
					return await Task.FromResult(true);
				}

				_logger.LogWarning("OTP validation failed: Invalid OTP for UserId: {UserId}", userId);
				return await Task.FromResult(false);
			}

			_logger.LogWarning("OTP validation failed: No OTP found or expired for UserId: {UserId}", userId);
			return await Task.FromResult(false);
		}

		private string GenerateOtp()
		{
			using var rng = RandomNumberGenerator.Create();
			var bytes = new byte[4];
			rng.GetBytes(bytes);
			uint raw = BitConverter.ToUInt32(bytes, 0);
			uint otp = raw % 1_000_000; // رقم من 000000 لغاية 999999

			return otp.ToString("D6"); // يحافظ على 6 خانات
		}
	}
}
