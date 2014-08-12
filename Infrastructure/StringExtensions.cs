using System;

namespace SWPCCBilling2
{
	public static class StringExtensions
	{
		public static string EmptyAsNull(this string text)
		{
			return String.IsNullOrEmpty(text)
				? null
				: text;
		}
	}
}

