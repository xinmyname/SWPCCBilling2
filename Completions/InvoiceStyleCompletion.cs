using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Completions
{
	public class InvoiceStyleCompletion : FilteredStringCompletion
	{
		public InvoiceStyleCompletion()
		{
			AllStrings.Add("Monthly");
			AllStrings.Add("Camp");
		}
	}
}
