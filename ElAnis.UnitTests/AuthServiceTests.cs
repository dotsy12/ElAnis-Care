using System.Security.Claims;
using ElAnis.DataAccess;
using ElAnis.DataAccess.Repositories.Interfaces;
using ElAnis.DataAccess.Services.Auth;
using ElAnis.DataAccess.Services.Email;
using ElAnis.DataAccess.Services.OTP;
using ElAnis.DataAccess.Services.Token;
using ElAnis.Entities.DTO.Account.Auth.Login;
using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.DTO.Account.Auth.ResetPassword;
using ElAnis.Entities.Models;
using ElAnis.Entities.Models.Auth.Identity;
using ElAnis.Entities.Models.Auth.UserTokens;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

namespace ElAnis.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IOTPService> _otpServiceMock;
        private readonly Mock<ITokenStoreService> _tokenStoreServiceMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IServiceProviderApplicationRepository> _applicationRepositoryMock;
        private readonly Mock<IGenericRepository<UserRefreshToken>> _refreshTokenRepositoryMock;
        private readonly ResponseHandler _responseHandler;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userManagerMock = MockUserManager();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();
            _otpServiceMock = new Mock<IOTPService>();
            _tokenStoreServiceMock = new Mock<ITokenStoreService>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _applicationRepositoryMock = new Mock<IServiceProviderApplicationRepository>();
         
            _refreshTokenRepositoryMock = new Mock<IGenericRepository<UserRefreshToken>>();
            _responseHandler = new ResponseHandler();

            // Setup UnitOfWork mocks
            _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.ServiceProviderApplications).Returns(_applicationRepositoryMock.Object);
          
            _unitOfWorkMock.Setup(u => u.Repository<UserRefreshToken>()).Returns(_refreshTokenRepositoryMock.Object);

            _authService = new AuthService(
                _userManagerMock.Object,
                _unitOfWorkMock.Object,
                _emailServiceMock.Object,
                _otpServiceMock.Object,
                _responseHandler,
                _tokenStoreServiceMock.Object,
                _loggerMock.Object
            );
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@test.com",
                PhoneNumber = "1234567890",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com", EmailConfirmed = true };
            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "WrongPassword"
            };

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal("Invalid password.", result.Message);
        }

        [Fact]
        public async Task LoginAsync_EmailNotConfirmed_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com", EmailConfirmed = false };
            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Email is not verified", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithTokens()
        {
            // Arrange
            var user = new User
            {
                Id = "user1",
                Email = "test@test.com",
                PhoneNumber = "1234567890",
                EmailConfirmed = true
            };
            var request = new LoginRequest
            {
                Email = "test@test.com",
                Password = "Password123!"
            };
            var roles = new List<string> { "USER" };
            var tokens = ("access_token", "refresh_token");

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync(user);
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(roles);
            _tokenStoreServiceMock.Setup(t => t.GenerateAndStoreTokensAsync(user.Id, user))
                .ReturnsAsync(tokens);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(user.Id, result.Data.Id);
            Assert.Equal(user.Email, result.Data.Email);
            Assert.Equal(tokens.Item1, result.Data.AccessToken);
            Assert.Equal(tokens.Item2, result.Data.RefreshToken);
            Assert.Equal("USER", result.Data.Role);
        }

        #endregion

        #region ForgotPasswordAsync Tests

        [Fact]
        public async Task ForgotPasswordAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new ForgetPasswordRequest
            {
                Email = "nonexistent@test.com",
                PhoneNumber = "1234567890"
            };

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.ForgotPasswordAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ValidUser_GeneratesOtpAndSendsEmail()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com" };
            var request = new ForgetPasswordRequest { Email = "test@test.com" };
            var otp = "123456";

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.GenerateAndStoreOtpAsync(user.Id))
                .ReturnsAsync(otp);
            _emailServiceMock.Setup(e => e.SendOtpEmailAsync(user, otp))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.ForgotPasswordAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(user.Id, result.Data.UserId);
            _otpServiceMock.Verify(o => o.GenerateAndStoreOtpAsync(user.Id), Times.Once);
            _emailServiceMock.Verify(e => e.SendOtpEmailAsync(user, otp), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordAsync_EmailSendFails_StillReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com" };
            var request = new ForgetPasswordRequest { Email = "test@test.com" };
            var otp = "123456";

            _userRepositoryMock.Setup(r => r.FindByEmailOrPhoneAsync(request.Email, request.PhoneNumber))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.GenerateAndStoreOtpAsync(user.Id))
                .ReturnsAsync(otp);
            _emailServiceMock.Setup(e => e.SendOtpEmailAsync(user, otp))
                .ThrowsAsync(new Exception("Email send failed"));

            // Act
            var result = await _authService.ForgotPasswordAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(user.Id, result.Data.UserId);
        }

        #endregion

        #region ResetPasswordAsync Tests

        [Fact]
        public async Task ResetPasswordAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new ResetPasswordRequest
            {
                UserId = "nonexistent",
                Otp = "123456",
                NewPassword = "NewPass123!"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ResetPasswordAsync_InvalidOtp_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com" };
            var request = new ResetPasswordRequest
            {
                UserId = user.Id,
                Otp = "wrong_otp",
                NewPassword = "NewPass123!"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.ValidateOtpAsync(request.UserId, request.Otp))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Invalid or expired OTP", result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_ResetFails_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com" };
            var request = new ResetPasswordRequest
            {
                UserId = user.Id,
                Otp = "123456",
                NewPassword = "weak"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.ValidateOtpAsync(request.UserId, request.Otp))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset_token");
            _userManagerMock.Setup(m => m.ResetPasswordAsync(user, "reset_token", request.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

            // Act
            var result = await _authService.ResetPasswordAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Password too weak", result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_Success_InvalidatesTokensAndReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com", PhoneNumber = "1234567890" };
            var request = new ResetPasswordRequest
            {
                UserId = user.Id,
                Otp = "123456",
                NewPassword = "NewPass123!"
            };
            var roles = new List<string> { "USER" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.ValidateOtpAsync(request.UserId, request.Otp))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset_token");
            _userManagerMock.Setup(m => m.ResetPasswordAsync(user, "reset_token", request.NewPassword))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(roles);
            _tokenStoreServiceMock.Setup(t => t.InvalidateOldTokensAsync(user.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.ResetPasswordAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(user.Id, result.Data.UserId);
            Assert.Equal(user.Email, result.Data.Email);
            _tokenStoreServiceMock.Verify(t => t.InvalidateOldTokensAsync(user.Id), Times.Once);
        }

        #endregion

        #region VerifyOtpAsync Tests

        [Fact]
        public async Task VerifyOtpAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new VerifyOtpRequest { UserId = "nonexistent", Otp = "123456" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.VerifyOtpAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task VerifyOtpAsync_EmailAlreadyConfirmed_ReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = "user1", EmailConfirmed = true };
            var request = new VerifyOtpRequest { UserId = user.Id, Otp = "123456" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.VerifyOtpAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("already verified", result.Message);
        }

        [Fact]
        public async Task VerifyOtpAsync_InvalidOtp_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = "user1", EmailConfirmed = false };
            var request = new VerifyOtpRequest { UserId = user.Id, Otp = "wrong" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.ValidateOtpAsync(request.UserId, request.Otp))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.VerifyOtpAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Invalid or expired OTP", result.Message);
        }

        [Fact]
        public async Task VerifyOtpAsync_UpdateFails_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Id = "user1", EmailConfirmed = false };
            var request = new VerifyOtpRequest { UserId = user.Id, Otp = "123456" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.ValidateOtpAsync(request.UserId, request.Otp))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            var result = await _authService.VerifyOtpAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task VerifyOtpAsync_Success_ConfirmsEmailAndReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = "user1", EmailConfirmed = false };
            var request = new VerifyOtpRequest { UserId = user.Id, Otp = "123456" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.ValidateOtpAsync(request.UserId, request.Otp))
                .ReturnsAsync(true);
            _userManagerMock.Setup(m => m.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.VerifyOtpAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(user.EmailConfirmed);
            Assert.Contains("verified successfully", result.Message);
        }

        #endregion

        #region ResendOtpAsync Tests

        [Fact]
        public async Task ResendOtpAsync_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var request = new ResendOtpRequest { UserId = "nonexistent" };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.ResendOtpAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ResendOtpAsync_EmailAlreadyVerified_ReturnsSuccess()
        {
            // Arrange
            var user = new User { Id = "user1", EmailConfirmed = true };
            var request = new ResendOtpRequest { UserId = user.Id };

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ResendOtpAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("already verified", result.Message);
        }

        [Fact]
        public async Task ResendOtpAsync_Success_GeneratesAndSendsOtp()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "test@test.com", EmailConfirmed = false };
            var request = new ResendOtpRequest { UserId = user.Id };
            var otp = "123456";

            _userManagerMock.Setup(m => m.FindByIdAsync(request.UserId))
                .ReturnsAsync(user);
            _otpServiceMock.Setup(o => o.GenerateAndStoreOtpAsync(user.Id))
                .ReturnsAsync(otp);
            _emailServiceMock.Setup(e => e.SendOtpEmailAsync(user, otp))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.ResendOtpAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("resent successfully", result.Message);
            _otpServiceMock.Verify(o => o.GenerateAndStoreOtpAsync(user.Id), Times.Once);
            _emailServiceMock.Verify(e => e.SendOtpEmailAsync(user, otp), Times.Once);
        }

        #endregion

        #region LogoutAsync Tests

        [Fact]
        public async Task LogoutAsync_NoUserId_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new ClaimsPrincipal();

            // Act
            var result = await _authService.LogoutAsync(claims);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task LogoutAsync_ValidUserId_InvalidatesTokens()
        {
            // Arrange
            var userId = "user1";
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _tokenStoreServiceMock.Setup(t => t.InvalidateOldTokensAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(claims);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Contains("Logged out successfully", result.Message);
            _tokenStoreServiceMock.Verify(t => t.InvalidateOldTokensAsync(userId), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ExceptionThrown_ReturnsServerError()
        {
            // Arrange
            var userId = "user1";
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _tokenStoreServiceMock.Setup(t => t.InvalidateOldTokensAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authService.LogoutAsync(claims);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        #endregion

        #region ChangePasswordAsync Tests

        [Fact]
        public async Task ChangePasswordAsync_NoUserId_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new ClaimsPrincipal(); // مفيش userId
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "OldPass123!",
                NewPassword = "NewPass123!"
            };

            // Act
            var result = await _authService.ChangePasswordAsync(claims, request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal("User not authenticated", result.Message);

            _tokenStoreServiceMock.Verify(t => t.InvalidateOldTokensAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region RegisterUserAsync Tests

        [Fact]
        public async Task RegisterUserAsync_EmailExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest { Email = "existing@test.com", Password = "Pass123!" };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Email is already registered", result.Message);
        }

        [Fact]
        public async Task RegisterUserAsync_PhoneExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "new@test.com",
                PhoneNumber = "1234567890",
                Password = "Pass123!"
            };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.IsPhoneExistsAsync(request.PhoneNumber))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Phone number is already registered", result.Message);
        }

        [Fact]
        public async Task RegisterUserAsync_CreateUserFails_ReturnsAndRollsBack()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "new@test.com",
                Password = "weak",
                FirstName = "Test",
                LastName = "User"
            };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));
            _unitOfWorkMock.Setup(u => u.RollbackAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Password too weak", result.Message);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_Success_CreatesUserAndSendsOtp()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "new@test.com",
                PhoneNumber = "1234567890",
                Password = "Pass123!",
                FirstName = "Test",
                LastName = "User"
            };
            var tokens = ("access_token", "refresh_token");
            var otp = "123456";

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.IsPhoneExistsAsync(request.PhoneNumber))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "USER"))
                .ReturnsAsync(IdentityResult.Success);
         

            _tokenStoreServiceMock.Setup(t => t.GenerateAndStoreTokensAsync(It.IsAny<string>(), It.IsAny<User>()))
                .ReturnsAsync(tokens);
            _otpServiceMock.Setup(o => o.GenerateAndStoreOtpAsync(It.IsAny<string>()))
                .ReturnsAsync(otp);
            _emailServiceMock.Setup(e => e.SendOtpEmailAsync(It.IsAny<User>(), otp))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(request.Email, result.Data.Email);
            Assert.Equal("USER", result.Data.Role);
            Assert.False(result.Data.IsEmailConfirmed);
            Assert.Equal(tokens.Item1, result.Data.AccessToken);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _emailServiceMock.Verify(e => e.SendOtpEmailAsync(It.IsAny<User>(), otp), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ExceptionDuringProcess_RollsBack()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "new@test.com",
                Password = "Pass123!"
            };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                .ThrowsAsync(new Exception("Database error"));
            _unitOfWorkMock.Setup(u => u.RollbackAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        #endregion

        #region RegisterServiceProviderAsync Tests

        [Fact]
        public async Task RegisterServiceProviderAsync_EmailExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterServiceProviderRequest { Email = "existing@test.com" };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterServiceProviderAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Email is already registered", result.Message);
        }

        [Fact]
        public async Task RegisterServiceProviderAsync_Success_CreatesApplicationWithPendingStatus()
        {
            // Arrange
            var request = new RegisterServiceProviderRequest
            {
                Email = "provider@test.com",
                PhoneNumber = "1234567890",
                Password = "Pass123!",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Street",
                DateOfBirth = DateTime.Now.AddYears(-25),
                Bio = "Experienced provider",
                NationalId = "12345",
                Experience = "5 years",
                HourlyRate = 50,
                SelectedCategoryIds = new List<Guid> { Guid.NewGuid() }
            };
            var otp = "123456";

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.IsPhoneExistsAsync(request.PhoneNumber))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "USER"))
                .ReturnsAsync(IdentityResult.Success);
       

            _otpServiceMock.Setup(o => o.GenerateAndStoreOtpAsync(It.IsAny<string>()))
                .ReturnsAsync(otp);
            _emailServiceMock.Setup(e => e.SendOtpEmailAsync(It.IsAny<User>(), otp))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterServiceProviderAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(ServiceProviderApplicationStatus.Pending, result.Data.Status);
            _applicationRepositoryMock.Verify(r => r.AddAsync(It.Is<ServiceProviderApplication>(
                a => a.Status == ServiceProviderApplicationStatus.Pending)), Times.Once);
        }

        #endregion

        #region CreateAdminAsync Tests

        [Fact]
        public async Task CreateAdminAsync_EmailExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new AdminRegisterRequest { Email = "admin@test.com" };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.CreateAdminAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Email is already registered", result.Message);
        }

        [Fact]
        public async Task CreateAdminAsync_Success_CreatesAdminWithConfirmedEmail()
        {
            // Arrange
            var request = new AdminRegisterRequest
            {
                Email = "admin@test.com",
                PhoneNumber = "1234567890",
                Password = "AdminPass123!",
                FirstName = "Admin",
                LastName = "User"
            };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.IsPhoneExistsAsync(request.PhoneNumber))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((user, pwd) => user.EmailConfirmed = true);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "ADMIN"))
                .ReturnsAsync(IdentityResult.Success);
            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.CreateAdminAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("ADMIN", result.Data.Role);
            Assert.True(result.Data.IsEmailConfirmed);
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "ADMIN"), Times.Once);
        }

        [Fact]
        public async Task CreateAdminAsync_ExceptionThrown_RollsBack()
        {
            // Arrange
            var request = new AdminRegisterRequest
            {
                Email = "admin@test.com",
                Password = "AdminPass123!"
            };

            _userRepositoryMock.Setup(r => r.IsEmailExistsAsync(request.Email))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), request.Password))
                .ThrowsAsync(new Exception("Database error"));
            _unitOfWorkMock.Setup(u => u.RollbackAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.CreateAdminAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_InvalidToken_ThrowsSecurityTokenException()
        {
            // Arrange
            var refreshToken = "invalid_token";

            _tokenStoreServiceMock.Setup(t => t.IsValidAsync(refreshToken))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Microsoft.IdentityModel.Tokens.SecurityTokenException>(
                async () => await _authService.RefreshTokenAsync(refreshToken));
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenNotInDatabase_ThrowsSecurityTokenException()
        {
            // Arrange
            var refreshToken = "valid_but_not_in_db";

            _tokenStoreServiceMock.Setup(t => t.IsValidAsync(refreshToken))
                .ReturnsAsync(true);
            _refreshTokenRepositoryMock.Setup(r => r.FindSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRefreshToken, bool>>>()))
                .ReturnsAsync((UserRefreshToken)null);

            // Act & Assert
            await Assert.ThrowsAsync<Microsoft.IdentityModel.Tokens.SecurityTokenException>(
                async () => await _authService.RefreshTokenAsync(refreshToken));
        }

        //[Fact]
        //public async Task RefreshTokenAsync_UserNotFound_ThrowsSecurityTokenException()
        //{
        //    // Arrange
        //    var refreshToken = "valid_token";
        //    var tokenEntry = new UserRefreshToken { UserId = Guid.Parse("12345678-1234-1234-1234-123456789012"), Token = refreshToken };

        //    _tokenStoreServiceMock.Setup(t => t.IsValidAsync(refreshToken))
        //        .ReturnsAsync(true);
        //    _refreshTokenRepositoryMock.Setup(r => r.FindSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRefreshToken, bool>>>()))
        //        .ReturnsAsync(tokenEntry);
        //    _userManagerMock.Setup(m => m.FindByIdAsync(tokenEntry.UserId.ToString()))
        //        .ReturnsAsync((User)null);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<Microsoft.IdentityModel.Tokens.SecurityTokenException>(
        //        async () => await _authService.RefreshTokenAsync(refreshToken));
        //}

        //[Fact]
        //public async Task RefreshTokenAsync_Success_GeneratesNewTokens()
        //{
        //    // Arrange
        //    var refreshToken = "valid_token";
        //    var userId = Guid.NewGuid();
        //    var user = new User { Id = userId.ToString(), Email = "test@test.com" };
        //    var tokenEntry = new UserRefreshToken { UserId = userId, Token = refreshToken };
        //    var newTokens = ("new_access_token", "new_refresh_token");

        //    _tokenStoreServiceMock.Setup(t => t.IsValidAsync(refreshToken))
        //        .ReturnsAsync(true);
        //    _refreshTokenRepositoryMock.Setup(r => r.FindSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRefreshToken, bool>>>()))
        //        .ReturnsAsync(tokenEntry);
        //    _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString()))
        //        .ReturnsAsync(user);
        //    _tokenStoreServiceMock.Setup(t => t.InvalidateOldTokensAsync(user.Id))
        //        .Returns(Task.CompletedTask);
        //    _tokenStoreServiceMock.Setup(t => t.GenerateAndStoreTokensAsync(user.Id, user))
        //        .ReturnsAsync(newTokens);
        //    _tokenStoreServiceMock.Setup(t => t.SaveRefreshTokenAsync(user.Id, newTokens.Item2))
        //        .Returns(Task.CompletedTask);

        //    // Act
        //    var result = await _authService.RefreshTokenAsync(refreshToken);

        //    // Assert
        //    Assert.True(result.Succeeded);
        //    Assert.Equal(newTokens.Item1, result.Data.AccessToken);
        //    Assert.Equal(newTokens.Item2, result.Data.RefreshToken);
        //    _tokenStoreServiceMock.Verify(t => t.InvalidateOldTokensAsync(user.Id), Times.Once);
        //    _tokenStoreServiceMock.Verify(t => t.SaveRefreshTokenAsync(user.Id, newTokens.Item2), Times.Once);
        //}

        #endregion
    }
}

