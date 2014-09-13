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

		public static string ToCardinalDayString(this DateTime date)
		{
			switch (date.Day)
			{
				case 1: 
				case 21: 
				case 31: 
					return String.Format("{0}st", date.Day);
				case 2: 
				case 22: 
					return String.Format("{0}nd", date.Day);
				case 3: 
				case 23: 
					return String.Format("{0}rd", date.Day);
			}

			return String.Format("{0}th", date.Day);
		}

		public static string ToCardinalDateString(this DateTime date)
		{
			return date.ToString("dddd, MMMMM ") + date.ToCardinalDayString();
		}
	}
}
