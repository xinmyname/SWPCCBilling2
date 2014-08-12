using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Completions
{
	public class FeeCompletion : FilteredStringCompletion
	{
		public FeeCompletion()
		{
			var feeStore = new FeeStore();

			foreach (Fee fee in feeStore.LoadAll())
				AllStrings.Add(fee.Code);
		}
	}
}
