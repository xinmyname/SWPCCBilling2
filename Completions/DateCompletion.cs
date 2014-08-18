using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Completions
{

	public class DateCompletion : FilteredStringCompletion
	{
		public DateCompletion()
		{
			AllStrings.Add("January");
			AllStrings.Add("February");
			AllStrings.Add("March");
			AllStrings.Add("April");
			AllStrings.Add("May");
			AllStrings.Add("June");
			AllStrings.Add("July");
			AllStrings.Add("August");
			AllStrings.Add("September");
			AllStrings.Add("October");
			AllStrings.Add("November");
			AllStrings.Add("December");
		}
	}
}
