using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Configurations;
using ElAnis.Utilities.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;

namespace ElAnis.DataAccess.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly ResponseHandler _responseHandler;
        private readonly StripeSettings _stripeSettings;

        public PaymentService(
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger,
            ResponseHandler responseHandler,
            IOptions<StripeSettings> stripeSettings)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _responseHandler = responseHandler;
            _stripeSettings = stripeSettings.Value;

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<Response<PaymentResponse>> CreateStripeCheckoutSessionAsync(
            CreatePaymentDto request,
            ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<PaymentResponse>("User not authenticated");

                var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(request.ServiceRequestId);
                if (serviceRequest == null)
                    return _responseHandler.NotFound<PaymentResponse>("Service request not found");

                if (serviceRequest.UserId != userId)
                    return _responseHandler.Forbidden<PaymentResponse>("You are not authorized to pay for this request");

                if (serviceRequest.Status != ServiceRequestStatus.Accepted)
                    return _responseHandler.BadRequest<PaymentResponse>("Request must be accepted before payment");

                var existingPayment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(request.ServiceRequestId);
                if (existingPayment != null && existingPayment.PaymentStatus == PaymentStatus.Completed)
                    return _responseHandler.BadRequest<PaymentResponse>("Payment already completed");

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "egp",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Service: {serviceRequest.Category.Name}",
                                    Description = $"Shift: {serviceRequest.ShiftType} on {serviceRequest.PreferredDate:yyyy-MM-dd}",
                                },
                                UnitAmount = (long)(serviceRequest.TotalPrice * 100),
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = $"{_stripeSettings.FrontendUrl}/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_stripeSettings.FrontendUrl}/payment/cancel?request_id={request.ServiceRequestId}",
                    ClientReferenceId = request.ServiceRequestId.ToString(),
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        Metadata = new Dictionary<string, string>
                        {
                            { "service_request_id", request.ServiceRequestId.ToString() },
                            { "user_id", userId }
                        }
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "service_request_id", request.ServiceRequestId.ToString() },
                        { "user_id", userId }
                    }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                if (existingPayment == null)
                {
                    var payment = new ElAnis.Entities.Models.Payment
                    {
                        ServiceRequestId = request.ServiceRequestId,
                        Amount = serviceRequest.TotalPrice,
                        PaymentMethod = ElAnis.Utilities.Enum.PaymentMethod.CreditCard,
                        PaymentStatus = PaymentStatus.Pending,
                        TransactionId = session.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Payments.AddAsync(payment);
                }
                else
                {
                    existingPayment.TransactionId = session.Id;
                    existingPayment.PaymentStatus = PaymentStatus.Pending;
                    _unitOfWork.Payments.Update(existingPayment);
                }

                serviceRequest.Status = ServiceRequestStatus.PaymentPending;
                _unitOfWork.ServiceRequests.Update(serviceRequest);

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"✅ Checkout session created: {session.Id} for request: {request.ServiceRequestId}");

                var response = new PaymentResponse
                {
                    Id = existingPayment?.Id ?? Guid.Empty,
                    ServiceRequestId = request.ServiceRequestId,
                    Amount = serviceRequest.TotalPrice,
                    PaymentMethod = ElAnis.Utilities.Enum.PaymentMethod.CreditCard,
                    PaymentStatus = PaymentStatus.Pending,
                    TransactionId = session.Id,
                    CheckoutUrl = session.Url,
                    CreatedAt = DateTime.UtcNow
                };

                return _responseHandler.Success(response, "Stripe checkout session created successfully");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error while creating checkout session");
                return _responseHandler.ServerError<PaymentResponse>($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe checkout session");
                return _responseHandler.ServerError<PaymentResponse>("Error creating payment session");
            }
        }

        public async Task<Response<PaymentResponse>> HandleStripeWebhookAsync(string json, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _stripeSettings.WebhookSecret,
                    throwOnApiVersionMismatch: false
                );

                _logger.LogInformation($"✅ Webhook received: {stripeEvent.Type}");

                switch (stripeEvent.Type)
                {
                    case EventTypes.CheckoutSessionCompleted:
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        break;

                    case EventTypes.CheckoutSessionExpired:
                        await HandleSessionExpired(stripeEvent);
                        break;

                    default:
                        _logger.LogInformation($"⚠️ Unhandled event: {stripeEvent.Type}");
                        break;
                }

                return _responseHandler.Success<PaymentResponse>(null, "Webhook processed");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "❌ Webhook verification failed");
                return _responseHandler.BadRequest<PaymentResponse>($"Webhook error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Webhook processing error");
                return _responseHandler.ServerError<PaymentResponse>("Error processing webhook");
            }
        }

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            try
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                {
                    _logger.LogWarning("⚠️ Invalid session data");
                    return;
                }

                _logger.LogInformation($"💳 Processing payment for session: {session.Id}");
                _logger.LogInformation($"📋 ClientReferenceId: {session.ClientReferenceId}");
                _logger.LogInformation($"💰 Payment status: {session.PaymentStatus}");

                // ✅ الحل: استخدام ClientReferenceId اللي فيه الـ ServiceRequestId
                if (string.IsNullOrEmpty(session.ClientReferenceId))
                {
                    _logger.LogError($"❌ No ClientReferenceId in session: {session.Id}");
                    return;
                }

                if (!Guid.TryParse(session.ClientReferenceId, out var serviceRequestId))
                {
                    _logger.LogError($"❌ Invalid ClientReferenceId: {session.ClientReferenceId}");
                    return;
                }

                // ✅ تحديث Payment
                var payment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(serviceRequestId);
                if (payment == null)
                {
                    _logger.LogError($"❌ Payment not found for request: {serviceRequestId}");
                    return;
                }

                _logger.LogInformation($"📊 Current payment status: {payment.PaymentStatus}");

                // ✅ تحديث الحالة
                payment.PaymentStatus = PaymentStatus.Completed;
                payment.PaidAt = DateTime.UtcNow;
                payment.TransactionId = session.PaymentIntentId ?? session.Id;
                payment.PaymentGatewayResponse = JsonSerializer.Serialize(session);

                _unitOfWork.Payments.Update(payment);

                // ✅ تحديث ServiceRequest
                var serviceRequest = await _unitOfWork.ServiceRequests.GetByIdAsync(serviceRequestId);
                if (serviceRequest != null)
                {
                    _logger.LogInformation($"📊 Current request status: {serviceRequest.Status}");

                    serviceRequest.Status = ServiceRequestStatus.Paid;
                    _unitOfWork.ServiceRequests.Update(serviceRequest);

                    _logger.LogInformation($"✅ Request status updated to: Paid");
                }
                else
                {
                    _logger.LogError($"❌ ServiceRequest not found: {serviceRequestId}");
                }

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"🎉 Payment completed successfully for request: {serviceRequestId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in HandleCheckoutSessionCompleted");
            }
        }

        private async Task HandleSessionExpired(Event stripeEvent)
        {
            try
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null) return;

                _logger.LogWarning($"⏱️ Session expired: {session.Id}");

                if (string.IsNullOrEmpty(session.ClientReferenceId) ||
                    !Guid.TryParse(session.ClientReferenceId, out var serviceRequestId))
                {
                    return;
                }

                var payment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(serviceRequestId);
                if (payment != null && payment.PaymentStatus == PaymentStatus.Pending)
                {
                    payment.PaymentStatus = PaymentStatus.Failed;
                    _unitOfWork.Payments.Update(payment);

                    var serviceRequest = await _unitOfWork.ServiceRequests.GetByIdAsync(serviceRequestId);
                    if (serviceRequest != null)
                    {
                        serviceRequest.Status = ServiceRequestStatus.Accepted;
                        _unitOfWork.ServiceRequests.Update(serviceRequest);
                    }

                    await _unitOfWork.CompleteAsync();
                    _logger.LogInformation($"✅ Session expiry handled for request: {serviceRequestId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in HandleSessionExpired");
            }
        }

        public async Task<Response<PaymentResponse>> GetPaymentByRequestIdAsync(Guid requestId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(requestId);
                if (payment == null)
                    return _responseHandler.NotFound<PaymentResponse>("Payment not found");

                var response = MapToResponse(payment);
                return _responseHandler.Success(response, "Payment retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment");
                return _responseHandler.ServerError<PaymentResponse>("Error retrieving payment");
            }
        }

        private PaymentResponse MapToResponse(ElAnis.Entities.Models.Payment payment)
        {
            return new PaymentResponse
            {
                Id = payment.Id,
                ServiceRequestId = payment.ServiceRequestId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt
            };
        }
    }
}