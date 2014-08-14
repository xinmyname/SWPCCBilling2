using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{
	public class HomeModule : NancyModule
	{
		public HomeModule()
		{
			Get["/"] = _ => View["Index"];
		}
	}
}

