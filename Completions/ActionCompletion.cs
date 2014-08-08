using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class ActionCompletion : FilteredStringCompletion
	{
		public ActionCompletion()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(Controller)))
					continue;

				foreach (MethodInfo methodInfo in type.GetMethods())
				{
					ActionAttribute actionAttr = methodInfo.GetCustomAttributes(false)
						.Where(attr => attr.GetType() == typeof(ActionAttribute))
						.Cast<ActionAttribute>()
						.SingleOrDefault();

					if (actionAttr != null)
						AllStrings.Add(actionAttr.Name);
				}
			}
		}
	}
}

