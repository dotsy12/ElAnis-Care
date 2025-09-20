namespace ElAnis.Entities.DTO.Account.Auth.Login
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
       // public string? Otp { get; set; } // Optional for initial login request, required for OTP verification

    }
}
