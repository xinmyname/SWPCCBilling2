using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Completions
{
	public class FamilyCompletion : FilteredStringCompletion
	{
		public FamilyCompletion()
		{
			AllStrings.Add("ALL");
			AllStrings.Add("RETURNING");
			AllStrings.Add("NEW");
			AllStrings.Add("GRADUATING");

			var familyStore = new FamilyStore();

			foreach (Family family in familyStore.LoadActive())
				AllStrings.Add(family.Name);
		}
	}

}
