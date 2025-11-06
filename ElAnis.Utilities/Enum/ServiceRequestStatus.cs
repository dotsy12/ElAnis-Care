using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Utilities.Enum
{
    // حالة طلب الخدمة
    public enum ServiceRequestStatus
    {
        Pending = 1,        // في انتظار قبول البروفايدر
        Accepted = 2,       // تم قبوله من البروفايدر
        PaymentPending = 3, // في انتظار الدفع
        Paid = 4,           // تم الدفع
        InProgress = 5,     // جاري التنفيذ
        Completed = 6,      // تم الإكمال
        Cancelled = 7,       // ملغي
        Rejected = 8        // مرفوض من البروفايدر
    }
}
