using System;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Text;
using SWPCCBilling2.Models;

namespace SWPCCBilling2
{
	class Bootstrapper
	{
		public static void Main(string[] args)
		{
			var cmdLineFactory = new CommandLineFactory(ActionMetaData.DefaultActionMetaData);

			foreach (CommandLine cmdLine in cmdLineFactory.Acquire())
			{
				if (cmdLine.HasErrors)
				{
					foreach (string error in cmdLine.Errors)
						Console.WriteLine(error);
				}
				else if (cmdLine.ActionInfo != null)
				{
					ActionInfo actionInfo = cmdLine.ActionInfo;
					object controller = Activator.CreateInstance(actionInfo.ControllerType);

					actionInfo.ActionMethod.Invoke(controller, cmdLine.Parameters);
				}
			}

			Console.WriteLine("Done!");
		}
	}
}
