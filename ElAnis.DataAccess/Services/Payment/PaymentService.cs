
using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using ElAnis.Utilities.Configurations;
using ElAnis.Utilities.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
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

            // تهيئة Stripe API Key
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        /// <summary>
        /// إنشاء Stripe Checkout Session
        /// </summary>
        public async Task<Response<PaymentResponse>> CreateStripeCheckoutSessionAsync(
            CreatePaymentDto request,
            ClaimsPrincipal userClaims)
        {
            try
            {
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return _responseHandler.Unauthorized<PaymentResponse>("User not authenticated");

                // 1️⃣ جلب الـ ServiceRequest
                var serviceRequest = await _unitOfWork.ServiceRequests.GetRequestWithDetailsAsync(request.ServiceRequestId);
                if (serviceRequest == null)
                    return _responseHandler.NotFound<PaymentResponse>("Service request not found");

                // 2️⃣ التحقق من أن الـ Request ملك اليوزر
                if (serviceRequest.UserId != userId)
                    return _responseHandler.Forbidden<PaymentResponse>("You are not authorized to pay for this request");

                // 3️⃣ التحقق من أن الـ Request تم قبوله
                if (serviceRequest.Status != ServiceRequestStatus.Accepted)
                    return _responseHandler.BadRequest<PaymentResponse>("Request must be accepted before payment");

                // 4️⃣ التحقق من عدم وجود دفع سابق
                var existingPayment = await _unitOfWork.Payments.GetByServiceRequestIdAsync(request.ServiceRequestId);
                if (existingPayment != null && existingPayment.PaymentStatus == PaymentStatus.Completed)
                    return _responseHandler.BadRequest<PaymentResponse>("Payment already completed");

                // 5️⃣ إنشاء Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "egp", // الجنيه المصري
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Service: {serviceRequest.Category.Name}",
                                    Description = $"Shift: {serviceRequest.ShiftType} on {serviceRequest.PreferredDate:yyyy-MM-dd}",
                                },
                                UnitAmount = (long)(serviceRequest.TotalPrice * 100), // Stripe يستخدم cents
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = $"http://localhost:3000/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"http://localhost:3000/payment/cancel?request_id={request.ServiceRequestId}",
                    ClientReferenceId = request.ServiceRequestId.ToString(), // ربط الـ Session بالـ Request
                    Metadata = new Dictionary<string, string>
                    {
                        { "service_request_id", request.ServiceRequestId.ToString() },
                        { "user_id", userId }
                    }
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                // 6️⃣ إنشاء أو تحديث Payment Entity
                if (existingPayment == null)
                {
                    var payment = new ElAnis.Entities.Models.Payment
                    {
                        ServiceRequestId = request.ServiceRequestId,
                        Amount = serviceRequest.TotalPrice,
                        PaymentMethod = ElAnis.Utilities.Enum.PaymentMethod.CreditCard, // Stripe = Credit Card
                        PaymentStatus = PaymentStatus.Pending,
                        TransactionId = session.Id, // Stripe Session ID
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

                // 7️⃣ تحديث ServiceRequest Status
                serviceRequest.Status = ServiceRequestStatus.PaymentPending;
                _unitOfWork.ServiceRequests.Update(serviceRequest);

                await _unitOfWork.CompleteAsync();

                // 8️⃣ إرجاع الـ Response
                var response = new PaymentResponse
                {
                    Id = existingPayment?.Id ?? Guid.Empty,
                    ServiceRequestId = request.ServiceRequestId,
                    Amount = serviceRequest.TotalPrice,
                    PaymentMethod = ElAnis.Utilities.Enum.PaymentMethod.CreditCard,
                    PaymentStatus = PaymentStatus.Pending,
                    TransactionId = session.Id,
                    CheckoutUrl = session.Url, // ⭐ الرابط اللي هنوجه اليوزر ليه
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

        /// <summary>
        /// معالجة Stripe Webhook (لما اليوزر يدفع)
        /// </summary>
        public async Task<Response<PaymentResponse>> HandleStripeWebhookAsync(string json, string signature)
        {
            try
            {
                // 1️⃣ التحقق من صحة الـ Webhook
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _stripeSettings.WebhookSecret
                );

                _logger.LogInformation($"Stripe webhook received: {stripeEvent.Type}");

                // 2️⃣ معالجة الـ Event
                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session == null)
                        return _responseHandler.BadRequest<PaymentResponse>("Invalid session data");

                    // 3️⃣ جلب الـ Payment من Database
                    var payment = await _unitOfWork.Payments.GetByTransactionIdAsync(session.Id);
                    if (payment == null)
                        return _responseHandler.NotFound<PaymentResponse>("Payment not found");

                    // 4️⃣ تحديث Payment Status
                    payment.PaymentStatus = PaymentStatus.Completed;
                    payment.PaidAt = DateTime.UtcNow;
                    payment.PaymentGatewayResponse = json; // حفظ الـ Response كاملة

                    _unitOfWork.Payments.Update(payment);

                    // 5️⃣ تحديث ServiceRequest Status
                    var serviceRequest = await _unitOfWork.ServiceRequests.GetByIdAsync(payment.ServiceRequestId);
                    if (serviceRequest != null)
                    {
                        serviceRequest.Status = ServiceRequestStatus.Paid;
                        _unitOfWork.ServiceRequests.Update(serviceRequest);
                    }

                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation($"Payment completed successfully for request {payment.ServiceRequestId}");

                    var response = MapToResponse(payment);
                    return _responseHandler.Success(response, "Payment completed successfully");
                }
                else if (stripeEvent.Type == EventTypes.CheckoutSessionExpired)
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        var payment = await _unitOfWork.Payments.GetByTransactionIdAsync(session.Id);
                        if (payment != null)
                        {
                            payment.PaymentStatus = PaymentStatus.Failed;
                            _unitOfWork.Payments.Update(payment);

                            var serviceRequest = await _unitOfWork.ServiceRequests.GetByIdAsync(payment.ServiceRequestId);
                            if (serviceRequest != null)
                            {
                                serviceRequest.Status = ServiceRequestStatus.Accepted; // رجوع للـ Accepted
                                _unitOfWork.ServiceRequests.Update(serviceRequest);
                            }

                            await _unitOfWork.CompleteAsync();
                        }
                    }
                }

                return _responseHandler.Success<PaymentResponse>(null, "Webhook processed");
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook verification failed");
                return _responseHandler.BadRequest<PaymentResponse>($"Webhook verification failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return _responseHandler.ServerError<PaymentResponse>("Error processing webhook");
            }
        }

        /// <summary>
        /// جلب حالة الدفع
        /// </summary>
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

        // Helper method
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