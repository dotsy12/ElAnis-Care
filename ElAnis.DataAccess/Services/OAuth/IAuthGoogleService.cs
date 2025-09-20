using ElAnis.Entities.DTO.Account.Auth.Register;
using ElAnis.Entities.Shared.Bases;

using Google.Apis.Auth;

namespace ElAnis.DataAccess.Services.OAuth
{
    public interface IAuthGoogleService
    {
        Task<Response<GoogleRegisterResponse>> AuthenticateWithGoogleAsync(string idToken);
        Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken);

    }
}
