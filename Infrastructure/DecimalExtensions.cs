using System;

namespace SWPCCBilling2
{

	public static class DecimalExtensions
	{
		public static string ToHtmlCurrency(this decimal value)
		{
			if (value >= 0)
				return value.ToString("C");

			return String.Format("<span class=\"neg\">{0:C}</span>", value);
		}
	}
}
