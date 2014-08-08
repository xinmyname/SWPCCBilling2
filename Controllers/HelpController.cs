using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class HelpController : Controller
	{
		public HelpController()
		{
		}

		[Action("help", ":topic?")]
		public void Help(string topic)
		{
		}
	}
}

