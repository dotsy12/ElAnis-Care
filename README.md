# Authentication System

A comprehensive ASP.NET Core authentication system with JWT tokens, OTP verification, Google OAuth integration, and modern security practices.

## Features

### Core Authentication
- **User Registration & Login** with email/phone number support
- **JWT Access & Refresh Tokens** for secure session management
- **Two-Factor Authentication (2FA)** with OTP via email
- **Password Management** (reset, change with current password verification)
- **Email Verification** required for account activation
- **Google OAuth Integration** for social login

### Security Features
- **Rate Limiting** for OTP requests to prevent abuse
- **Token Invalidation** on logout and password changes
- **Secure Password Requirements** (uppercase, lowercase, digits, special characters)
- **Claims-based Authorization** with role management
- **Redis-based OTP Storage** with automatic expiration

### Additional Services
- **Email Service** with HTML templates using FluentEmail
- **Image Upload Service** with Cloudinary integration
- **Comprehensive Logging** with Serilog
- **FluentValidation** for request validation
- **Response Handler Pattern** for consistent API responses

### Unit Testing 
 - **Note :** Unit tests for the endpoints have been implemented. Unit tests for the services are planned and will be added next.

## Architecture

### Project Structure
```
Ecommerce/
├── DataAccess/
│   ├── ApplicationContext/
│   └── Services/
│       ├── Auth/           # Authentication service
│       ├── Email/          # Email service
│       ├── OAuth/          # Google OAuth service
│       ├── OTP/            # OTP generation & validation
│       ├── Token/          # JWT token management
│       └── ImageUploading/ # Cloudinary image uploads
├── Entities/
│   ├── DTO/               # Data Transfer Objects
│   ├── Models/            # Entity models
│   └── Shared/            # Shared base classes
└── API/
    ├── Controllers/       # API controllers
    ├── Extensions/        # Service registration extensions
    └── Validators/        # FluentValidation validators
```

### Design Patterns
- **Repository Pattern** with Entity Framework Core
- **Options Pattern** for configuration management
- **Dependency Injection** throughout the application
- **Response Handler Pattern** for consistent API responses
- **Service Layer Pattern** for business logic separation

## Technology Stack

### Backend
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **ASP.NET Core Identity** - User management and authentication
- **JWT Bearer Authentication** - Stateless authentication
- **Redis** - OTP storage and caching
- **FluentEmail** - Email service abstraction
- **FluentValidation** - Request validation
- **Serilog** - Structured logging

### External Services
- **Google APIs** - OAuth authentication
- **Cloudinary** - Image hosting and management
- **SMTP** - Email delivery

## Configuration

### Required Settings

```json
{
  "JWT": {
    "SigningKey": "your-256-bit-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  },
  "GoogleAuth": {
    "ClientId": "your-google-client-id"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password"
  }
}
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login with OTP
- `POST /api/auth/verify-otp` - Email verification
- `POST /api/auth/resend-otp` - Resend verification OTP
- `POST /api/auth/forgot-password` - Password reset request
- `POST /api/auth/reset-password` - Reset password with OTP
- `POST /api/auth/change-password` - Change password (authenticated)
- `POST /api/auth/logout` - User logout
- `POST /api/auth/refresh` - Refresh access token

### OAuth
- `POST /api/auth/google` - Google OAuth authentication

## Key Services

### AuthService
Handles all authentication operations including registration, login, password management, and OTP verification.

### TokenStoreService
Manages JWT token generation, refresh token storage, and token validation using secure practices.

### OTPService
Generates cryptographically secure 6-digit OTPs with Redis storage and automatic expiration.

### EmailService
Sends templated emails with OTP codes using FluentEmail with HTML templates.

### AuthGoogleService
Integrates Google OAuth for social authentication with automatic user creation.

## Security Considerations

### Password Security
- Minimum 8 characters with complexity requirements
- Uses ASP.NET Core Identity's secure password hashing
- Password reset requires OTP verification

### Token Security
- Short-lived access tokens (7 days)
- Secure refresh token rotation
- Automatic token invalidation on security events

### OTP Security
- 6-digit cryptographically secure random generation
- 5-minute expiration window
- Single-use tokens with automatic cleanup
- Rate limiting to prevent brute force attacks

### Additional Security
- HTTPS enforcement
- CORS configuration
- Input validation with FluentValidation
- Structured logging for security monitoring

## Getting Started

### Prerequisites
- .NET 6.0 or later
- SQL Server or compatible database
- Redis server
- SMTP server access
- Google Developer Console project (for OAuth)
- Cloudinary account (for image uploads)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ecommerce-auth
   ```

2. **Configure settings**
   - Update `appsettings.json` with your configuration values
   - Ensure all required services are properly configured

3. **Database setup**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

### Docker Support
```dockerfile
# Example Dockerfile structure
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# Build steps...
```

## Usage Examples

### User Registration Flow
1. User submits registration form
2. System creates user account (unverified)
3. OTP sent via email
4. User verifies email with OTP
5. Account becomes active

### Login Flow
1. User submits credentials
2. System validates credentials
3. If valid, OTP sent via email
4. User submits OTP
5. System issues JWT tokens

### Password Reset Flow
1. User requests password reset
2. System sends OTP via email
3. User submits OTP + new password
4. Password updated, all tokens invalidated

## Rate Limiting

OTP requests are rate-limited to prevent abuse:
- **3 requests per minute** per IP address
- Configurable via the `AddResendOtpRateLimiter` extension

## Error Handling

The system uses a consistent response pattern:
```json
{
  "isSuccess": true/false,
  "message": "Description of result",
  "data": { /* Response data */ },
  "errors": [ /* List of errors if any */ ]
}
```

## Logging

Comprehensive logging with Serilog includes:
- Authentication attempts and results
- OTP generation and validation
- Token operations
- Error tracking with correlation IDs
- Security events monitoring

## Contributing

1. Follow the established patterns and architecture
2. Include comprehensive logging for new features
3. Add appropriate validation for all inputs
4. Write unit tests for new services
5. Update documentation for API changes

## License

This project is licensed under the MIT License - see the LICENSE file for details.
"# ElAnis-Care" 
