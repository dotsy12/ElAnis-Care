using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAnis.Utilities.Enum
{
	public enum ServiceProviderStatus
	{
		Pending = 1,    // في انتظار الموافقة
		Approved = 2,   // مقبول
		Rejected = 3,   // مرفوض
		Suspended = 4   // متوقف
	}
}
