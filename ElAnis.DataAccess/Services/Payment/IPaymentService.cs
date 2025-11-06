using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using System.Security.Claims;

namespace ElAnis.DataAccess.Services.Payment
{
    public interface IPaymentService
    {
        Task<Response<PaymentResponse>> CreateStripeCheckoutSessionAsync(
            CreatePaymentDto request,
            ClaimsPrincipal userClaims);

        Task<Response<PaymentResponse>> HandleStripeWebhookAsync(string json, string signature);

        Task<Response<PaymentResponse>> GetPaymentByRequestIdAsync(Guid requestId);
    }
}