using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

using StackExchange.Redis;

namespace ElAnis.DataAccess.Services.OTP
{
	public class OTPServiceRedis : IOTPService
	{
		private readonly IDatabase _redis;
		private readonly ILogger<OTPServiceRedis> _logger;


		public OTPServiceRedis(IConnectionMultiplexer redis, ILogger<OTPServiceRedis> logger)
		{
			_redis = redis.GetDatabase();
			_logger = logger;
		}

		public async Task<string> GenerateAndStoreOtpAsync(string userId)
		{
			var otp = GenerateOtp();

			bool success = await _redis.StringSetAsync($"otp:{userId}", otp, TimeSpan.FromMinutes(5));
			if (success)
				_logger.LogInformation("OTP generated and stored for UserId: {UserId}. Expiry: 5 Minutes", userId);
			else
				_logger.LogWarning("Failed to store OTP in Redis for UserId: {UserId}", userId);

			return otp;
		}

		public async Task<bool> ValidateOtpAsync(string userId, string otp)
		{
			var storedOtp = await _redis.StringGetAsync($"otp:{userId}");

			if (storedOtp.IsNullOrEmpty)
			{
				_logger.LogWarning("OTP validation failed: No OTP found or expired for UserId: {UserId}", userId);
				return false;
			}

			bool isValid = storedOtp == otp;

			if (isValid)
			{
				await _redis.KeyDeleteAsync($"otp:{userId}");
				_logger.LogInformation("OTP validated successfully for UserId: {UserId}", userId);
			}
			else
			{
				_logger.LogWarning("OTP validation failed: Invalid OTP for UserId: {UserId}", userId);
			}

			return isValid;
		}

		private string GenerateOtp()
		{
			using var rng = RandomNumberGenerator.Create();
			var bytes = new byte[4];
			rng.GetBytes(bytes);
			uint raw = BitConverter.ToUInt32(bytes, 0);
			uint otp = raw % 1_000_000; // number from 0 to 999999

			// if number < 6 digits -> ToString("6") will add zeros on left,right
			// otp = 123 - result = 000123
			// otp = 987654 - result = 987654
			return otp.ToString("D6");
		}
	}
}
