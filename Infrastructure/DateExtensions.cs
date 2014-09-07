using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2
{
	public static class DateExtensions
	{
		public static DateTime NextMonth(this DateTime month)
		{
			var nextMonth = month.AddMonths(1);
			return new DateTime(nextMonth.Year, nextMonth.Month, 1);
		}
	}
	
}
