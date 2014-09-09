using System;

namespace SWPCCBilling2
{

	public static class DecimalExtensions
	{
		public static string ToHtmlCurrency(this decimal value)
		{
			if (value > 0)
				return value.ToString("C");

			if (value == 0)
				return String.Format("<span class=\"zero\">{0:C}</span>", value);

			return String.Format("<span class=\"neg\">{0:C}</span>", value);
		}
	}
}
