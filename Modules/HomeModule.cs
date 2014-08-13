using System;
using Nancy;

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

