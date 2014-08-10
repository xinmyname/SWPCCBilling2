using System;
using SWPCCBilling2.Infrastructure;
using System.Runtime.InteropServices;

namespace SWPCCBilling2
{
	public class HelpController : Controller
	{
		public HelpController()
		{
		}

		[Action("help", "action-name (or nothing to see list of actions)")]
		public void Help([Optional][CompleteWith(typeof(ActionCompletion))]string action)
		{
			if (action == null)
				Console.WriteLine("All the halps!");
			else
				Console.WriteLine("Some specific halp for {0}.", action);
		}
	}
}

