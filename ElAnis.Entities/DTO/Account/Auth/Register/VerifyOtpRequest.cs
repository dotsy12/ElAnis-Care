namespace ElAnis.Entities.DTO.Account.Auth.Register
{
    public class VerifyOtpRequest
    {
        public string UserId { get; set; }
        public string Otp { get; set; }
    }
}
