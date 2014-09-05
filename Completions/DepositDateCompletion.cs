using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Completions
{
	public class DepositDateCompletion : FilteredStringCompletion
	{
		public DepositDateCompletion()
		{
			AllStrings.Add("PENDING");

			var depositStore = new DepositStore();

			foreach (Deposit deposit in depositStore.LoadAll())
				AllStrings.Add(deposit.Date.ToString("d"));
		}
	}

}
