using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Utilities.Enum
{
    // حالة الدفع
    public enum PaymentStatus
    {
        Pending = 1,    // في انتظار الدفع
        Paid = 2,       // تم الدفع
        Failed = 3,     // فشل
        Refunded = 4    // تم الاسترجاع
    }
}
