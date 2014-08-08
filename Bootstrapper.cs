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
			var cmdLineFactory = new CommandLineFactory();

			foreach (CommandLine cmdLine in cmdLineFactory.Acquire())
			{
			}

			Console.WriteLine("Done!");
		}
	}
}
