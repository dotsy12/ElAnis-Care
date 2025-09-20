using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Utilities.Enum
{
	public enum ServiceProviderApplicationStatus
	{
		Pending = 1,    // في انتظار المراجعة
		UnderReview = 2, // تحت المراجعة
		Approved = 3,   // مقبول
		Rejected = 4,   // مرفوض
		RequiresMoreInfo = 5 // يحتاج معلومات إضافية
	}
}
