using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class HelpController : Controller
	{
		public HelpController()
		{
		}

		[Action("help", ":action?")]
		public void Help(string action)
		{
			Console.WriteLine("Halp!!");
		}
	}
}

