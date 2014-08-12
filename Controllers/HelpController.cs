using System;
using SWPCCBilling2.Infrastructure;
using System.Runtime.InteropServices;
using SWPCCBilling2.Completions;

namespace SWPCCBilling2.Controllers
{
	public class HelpController : Controller
	{
		public HelpController()
		{
		}

		[Action("help", "action-name (or nothing to see list of actions)")]
		public void Help([Optional][CompleteWith(typeof(ActionCompletion))]string action)
		{
			ActionMetaData actionMetaData = ActionMetaData.DefaultActionMetaData;

			if (action == null)
			{
				Console.WriteLine("You can request help on the following actions:\n");
				foreach (ActionInfo actionInfo in actionMetaData.GetAllActions())
					Console.WriteLine("    {0}", actionInfo.Name);
			}
			else
			{
				ActionInfo actionInfo = actionMetaData.GetAction(action);

				if (actionInfo == null)
					Console.WriteLine("Action \"{0}\" does not exist. Type \"help\" for a list of actions.", action);
				else
					Console.WriteLine(actionInfo.HelpText);
			}
		}
	}
}

