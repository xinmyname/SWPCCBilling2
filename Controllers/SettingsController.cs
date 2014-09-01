using System;
using SWPCCBilling2.Infrastructure;
using System.Runtime.InteropServices;
using SWPCCBilling2.Completions;

namespace SWPCCBilling2.Controllers
{
	public class SettingsController : Controller
	{
		private readonly DateFactory _dateFactory;

		public SettingsController()
		{
			_dateFactory = DateFactory.DefaultDateFactory;
		}

		[Action("set-database", ":name")]
		public void SetDatabase(string name)
		{
			throw new NotImplementedException();
		}

		[Action("set-email-server", ":host")]
		public void SetEmailServer(string host)
		{
			throw new NotImplementedException();
		}

		[Action("set-email-port", ":port")]
		public void SetEmailPort(int port)
		{
			throw new NotImplementedException();
		}

		[Action("set-email-secure", ":value")]
		public void SetEmailSecure(bool value)
		{
			throw new NotImplementedException();
		}

		[Action("set-email-from", ":address")]
		public void SetEmailFrom(string address)
		{
			throw new NotImplementedException();
		}

		[Action("set-date")]
		public void SetDate(
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? fakeDate)
		{
			if (fakeDate.HasValue)
				_dateFactory.SetFakeDate(fakeDate.Value);
			else
				_dateFactory.ClearFakeDate();
		}

		[Action("get-date")]
		public void GetDate()
		{
			Console.WriteLine(_dateFactory.Now());
		}
	}
}

