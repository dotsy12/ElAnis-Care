using ElAnis.DataAccess.Services.Payment;
using ElAnis.Entities.DTO.Payment;
using ElAnis.Entities.Shared.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAnis.API.Controllers
{
    /// <summary>
    /// Payment Controller - يدير عمليات الدفع باستخدام Stripe
    /// 
    /// 🎯 الوظائف الرئيسية:
    /// 1. إنشاء Stripe Checkout Session (عشان اليوزر يدفع)
    /// 2. استقبال Webhooks من Stripe (تأكيد الدفع)
    /// 3. عرض حالة الدفع
    /// 
    /// 📌 ملحوظات مهمة:
    /// - كل الـ endpoints تستخدم Test Mode في Stripe
    /// - الـ Webhook endpoint لازم يكون public (بدون Authorization)
    /// - لازم تسجل الـ Webhook URL في Stripe Dashboard
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ResponseHandler _responseHandler;

        public PaymentsController(
            IPaymentService paymentService,
            ResponseHandler responseHandler)
        {
            _paymentService = paymentService;
            _responseHandler = responseHandler;
        }

        /// <summary>
        /// ✅ إنشاء جلسة دفع Stripe
        /// 
        /// 🎯 الوظيفة:
        /// - ينشئ Stripe Checkout Session
        /// - يحفظ Payment في الداتابيز بحالة Pending
        /// - يرجع checkoutUrl اللي اليوزر يروح ليه يدفع
        /// 
        /// 📝 متى تستخدمها:
        /// - لما اليوزر يضغط على زرار "Pay Now" في صفحة الـ Request
        /// 
        /// 🔐 Authorization:
        /// - مطلوب تكون مسجل دخول
        /// - لازم تكون صاحب الـ ServiceRequest
        /// 
        /// 📊 Request Example:
        /// POST /api/Payments/create-checkout
        /// {
        ///   "serviceRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        /// }
        /// 
        /// ✅ Response Example (Success):
        /// {
        ///   "succeeded": true,
        ///   "statusCode": 200,
        ///   "message": "Stripe checkout session created successfully",
        ///   "data": {
        ///     "id": "payment-guid",
        ///     "serviceRequestId": "request-guid",
        ///     "amount": 150.00,
        ///     "paymentMethod": 2,
        ///     "paymentStatus": 0,
        ///     "transactionId": "cs_test_abc123...",
        ///     "checkoutUrl": "https://checkout.stripe.com/c/pay/cs_test_abc123...",
        ///     "createdAt": "2025-11-02T10:00:00Z"
        ///   }
        /// }
        /// 
        /// ❌ Error Responses:
        /// - 401: User not authenticated
        /// - 403: You are not authorized to pay for this request
        /// - 404: Service request not found
        /// - 400: Request must be accepted before payment
        /// - 400: Payment already completed
        /// - 500: Stripe error or server error
        /// </summary>
        /// <param name="request">يحتوي على ServiceRequestId</param>
        /// <returns>Payment response مع checkoutUrl</returns>
        [HttpPost("create-checkout")]
        [Authorize]
        [ProducesResponseType(typeof(Response<PaymentResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 403)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreatePaymentDto request)
        {
            if (request.ServiceRequestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid service request ID"));

            var response = await _paymentService.CreateStripeCheckoutSessionAsync(request, User);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// 🔔 Stripe Webhook Endpoint - يستقبل إشعارات من Stripe
        /// 
        /// 🎯 الوظيفة:
        /// - يستقبل Events من Stripe لما الدفع ينجح أو يفشل
        /// - يحدث حالة Payment في الداتابيز
        /// - يحدث حالة ServiceRequest (من Accepted لـ Paid)
        /// 
        /// ⚙️ Events المعالجة:
        /// 1. checkout.session.completed → الدفع نجح
        /// 2. checkout.session.expired → الـ Session انتهت صلاحيته
        /// 
        /// 🔒 Security:
        /// - يتحقق من صحة الـ Webhook باستخدام Stripe Signature
        /// - لازم الـ endpoint ده يكون مسجل في Stripe Dashboard
        /// 
        /// 📍 Configuration في Stripe Dashboard:
        /// 1. اذهب لـ: Developers → Webhooks
        /// 2. اضغط "Add endpoint"
        /// 3. URL: https://yourdomain.com/api/Payments/webhook
        /// 4. Events: checkout.session.completed, checkout.session.expired
        /// 5. انسخ الـ Signing Secret وحطه في appsettings.json
        /// 
        /// 🧪 Testing:
        /// - استخدم Stripe CLI للـ Testing المحلي:
        ///   stripe listen --forward-to localhost:5000/api/Payments/webhook
        /// 
        /// ⚠️ ملحوظة مهمة:
        /// - هذا الـ endpoint PUBLIC (بدون [Authorize])
        /// - لأن Stripe بيبعت الـ Webhook مش اليوزر
        /// - التحقق من الصحة بيتم عن طريق الـ Signature
        /// 
        /// ✅ Response (Success): 200 OK
        /// ❌ Response (Fail): 400 Bad Request
        /// </summary>
        /// <returns>200 OK أو 400 Bad Request</returns>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing Stripe signature");

            var response = await _paymentService.HandleStripeWebhookAsync(json, signature);

            if (response.Succeeded)
                return Ok();
            else
                return BadRequest(response.Message);
        }

        /// <summary>
        /// 📊 جلب حالة الدفع لـ ServiceRequest معين
        /// 
        /// 🎯 الوظيفة:
        /// - يجيب تفاصيل Payment المرتبط بـ ServiceRequest
        /// - يظهر حالة الدفع (Pending, Completed, Failed)
        /// 
        /// 📝 متى تستخدمها:
        /// - لما اليوزر يرجع من Stripe Checkout
        /// - لمعرفة إذا الدفع تم بنجاح ولا لأ
        /// 
        /// 🔐 Authorization:
        /// - مطلوب تكون مسجل دخول
        /// 
        /// 📊 Request Example:
        /// GET /api/Payments/request/3fa85f64-5717-4562-b3fc-2c963f66afa6
        /// 
        /// ✅ Response Example:
        /// {
        ///   "succeeded": true,
        ///   "statusCode": 200,
        ///   "message": "Payment retrieved successfully",
        ///   "data": {
        ///     "id": "payment-guid",
        ///     "serviceRequestId": "request-guid",
        ///     "amount": 150.00,
        ///     "paymentMethod": 2,
        ///     "paymentStatus": 1, // 1 = Completed
        ///     "transactionId": "cs_test_abc123",
        ///     "createdAt": "2025-11-02T10:00:00Z",
        ///     "paidAt": "2025-11-02T10:05:00Z"
        ///   }
        /// }
        /// 
        /// ❌ Error Responses:
        /// - 401: Unauthorized
        /// - 404: Payment not found
        /// - 500: Server error
        /// </summary>
        /// <param name="requestId">ServiceRequest ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("request/{requestId}")]
        [Authorize]
        [ProducesResponseType(typeof(Response<PaymentResponse>), 200)]
        [ProducesResponseType(typeof(Response<object>), 400)]
        [ProducesResponseType(typeof(Response<object>), 401)]
        [ProducesResponseType(typeof(Response<object>), 404)]
        [ProducesResponseType(typeof(Response<object>), 500)]
        public async Task<IActionResult> GetPaymentByRequestId(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return BadRequest(_responseHandler.BadRequest<object>("Invalid request ID"));

            var response = await _paymentService.GetPaymentByRequestIdAsync(requestId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}