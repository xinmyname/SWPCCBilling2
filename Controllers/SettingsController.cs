using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class SettingsController : Controller
	{
		public SettingsController()
		{
		}

		[Action("set-database", ":name")]
		public void SetDatabase(string name)
		{
		}

		[Action("set-email-server", ":host")]
		public void SetEmailServer(string host)
		{
		}

		[Action("set-email-port", ":port")]
		public void SetEmailPort(int port)
		{
		}

		[Action("set-email-secure", ":value")]
		public void SetEmailSecure(bool value)
		{
		}

		[Action("set-email-from", ":address")]
		public void SetEmailFrom(string address)
		{
		}
	}
}

