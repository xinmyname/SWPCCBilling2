using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2
{
	public class DateFactory
	{
		private static DateFactory _defaultDateFactory;

		public static DateFactory DefaultDateFactory
		{
			get 
			{
				if (_defaultDateFactory == null)
					_defaultDateFactory = new DateFactory();

				return _defaultDateFactory;
			}
		}
	
		private DateTime? _fakeDate;

		public DateFactory()
		{
			_fakeDate = null;
		}

		public void SetFakeDate(DateTime fakeDate)
		{
			_fakeDate = fakeDate;
		}

		public void ClearFakeDate()
		{
			_fakeDate = null;
		}

		public DateTime Now()
		{
			if (_fakeDate != null)
			{
				DateTime fakeDate = _fakeDate.Value;
				DateTime actualDate = DateTime.Now;

				return new DateTime(fakeDate.Year, fakeDate.Month, fakeDate.Day,
					actualDate.Hour, actualDate.Minute, actualDate.Second);
			}

			return DateTime.Now;
		}

		public DateTime DateFromText(string text)
		{
			DateTime now = Now();

			switch (text.ToLower().Substring(0, 3))
			{
				case "jan": return MonthClosestTo(now, 1);
				case "feb": return MonthClosestTo(now, 2);
				case "mar": return MonthClosestTo(now, 3);
				case "apr": return MonthClosestTo(now, 4);
				case "may": return MonthClosestTo(now, 5);
				case "jun": return MonthClosestTo(now, 6);
				case "jul": return MonthClosestTo(now, 7);
				case "aug": return MonthClosestTo(now, 8);
				case "sep": return MonthClosestTo(now, 9);
				case "oct": return MonthClosestTo(now, 10);
				case "nov": return MonthClosestTo(now, 11);
				case "dec": return MonthClosestTo(now, 12);
			}

			return Convert.ToDateTime(text);
		}

		public DateTime MonthClosestTo(DateTime date, int month)
		{
			DateTime lastYear = new DateTime(date.Year - 1, month, 1);
			DateTime thisYear = new DateTime(date.Year, month, 1);
			DateTime nextYear = new DateTime(date.Year + 1, month, 1);

			var deltas = new Dictionary<long, DateTime>();

			deltas[Math.Abs((lastYear - date).Ticks)] = lastYear;
			deltas[Math.Abs((thisYear - date).Ticks)] = thisYear;
			deltas[Math.Abs((nextYear - date).Ticks)] = nextYear;

			long smallestDelta = deltas.Keys.Min();

			return deltas[smallestDelta];
		}

		public DateTime GetInvoiceDate(DateTime? date)
		{
			DateTime now = Now();
			DateTime nextMonth = now.AddMonths(1);

			DateTime invoiceDate = date != null 
				? date.Value
				: new DateTime(nextMonth.Year, nextMonth.Month, 1);

			return invoiceDate;
		}

		public DateTime GetReportDate(DateTime? date)
		{
			DateTime now = Now();

			DateTime reportDate = date != null 
				? date.Value
				: new DateTime(now.Year, now.Month, 1);

			return reportDate;
		}

		public string CardinalDay(DateTime date)
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
	}
}
