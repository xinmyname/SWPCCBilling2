using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Completions
{
	public class ActionCompletion : FilteredStringCompletion
	{
		public ActionCompletion()
		{
			foreach (ActionInfo actionInfo in ActionMetaData.DefaultActionMetaData.GetAllActions())
				AllStrings.Add(actionInfo.Name);
		}
	}
}

