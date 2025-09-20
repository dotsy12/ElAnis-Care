using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Utilities.Enum
{
	namespace ElAnis.Utilities.Enum
	{
		public enum NotificationType
		{
			ServiceRequest = 1,      // طلب خدمة جديد
			ServiceAccepted = 2,     // قبول الخدمة
			ServiceRejected = 3,     // رفض الخدمة
			ServiceCompleted = 4,    // اكتمال الخدمة
			PaymentReceived = 5,     // استلام الدفع
			ReviewReceived = 6,      // استلام تقييم
			ApplicationApproved = 7, // موافقة على الطلب
			ApplicationRejected = 8, // رفض الطلب
			SystemUpdate = 9,        // تحديث النظام
			General = 10            // عام
		}
	}
}
