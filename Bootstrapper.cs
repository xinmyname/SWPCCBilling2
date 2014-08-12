using System;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Text;
using SWPCCBilling2.Models;
using System.Reflection;
using System.Linq;

namespace SWPCCBilling2
{
	class Bootstrapper
	{
		public static void Main(string[] args)
		{
			var cmdLineFactory = new CommandLineFactory(ActionMetaData.DefaultActionMetaData);

			string combinedArgs = null;

			if (args.Length > 0)
				combinedArgs = args.Aggregate((cur, next) => cur + " " + next);

			foreach (CommandLine cmdLine in cmdLineFactory.Acquire(combinedArgs))
			{
				if (cmdLine.HasErrors)
				{
					foreach (string error in cmdLine.Errors)
						Console.WriteLine("ERROR: {0}", error);
				}
				else if (cmdLine.ActionInfo != null)
				{
					ActionInfo actionInfo = cmdLine.ActionInfo;
					object controller = Activator.CreateInstance(actionInfo.ControllerType);

					try
					{
						actionInfo.ActionMethod.Invoke(controller, cmdLine.Parameters);
					}
					catch (TargetInvocationException ex)
					{
						Console.WriteLine("ERROR: {0}", ex.InnerException.Message);
					}
					catch (Exception ex)
					{
						Console.WriteLine("ERROR: {0}", ex.Message);
					}

				}
			}

			Console.WriteLine("Done!");
		}
	}
}
