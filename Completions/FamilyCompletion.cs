using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class FamilyCompletion : FilteredStringCompletion
	{
		public FamilyCompletion()
		{
			AllStrings.Add("ALL");
			AllStrings.Add("RETURNING");
			AllStrings.Add("NEW");
			AllStrings.Add("GRADUATING");
		}
	}

}
