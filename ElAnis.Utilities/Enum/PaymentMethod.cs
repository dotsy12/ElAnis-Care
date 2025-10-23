using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Utilities.Enum
{
    // طريقة الدفع
    public enum PaymentMethod
    {
        Cash = 1,           // كاش عند الاستلام
        CreditCard = 2,     // بطاقة ائتمان
        VodafoneCash = 3,   // فودافون كاش
        InstaPay = 4,       // انستاباي
        BankTransfer = 5    // تحويل بنكي
    }
}
