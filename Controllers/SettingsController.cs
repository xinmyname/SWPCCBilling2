using System;
using SWPCCBilling2.Infrastructure;
using System.Runtime.InteropServices;
using SWPCCBilling2.Completions;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Controllers
{
	public class SettingsController : Controller
	{
		private readonly DateFactory _dateFactory;
		private readonly SettingsStore _settingsStore;

		public SettingsController()
		{
			_dateFactory = DateFactory.DefaultDateFactory;
			_settingsStore = SettingsStore.DefaultSettingsStore;
		}

		[Action("set-database", ":name")]
		public void SetDatabase(string name)
		{
			Settings settings = _settingsStore.Load();
			settings.DatabaseName = name + ".sqlite";
			_settingsStore.Save(settings);
		}

		[Action("set-email-server", ":host")]
		public void SetEmailServer(string host)
		{
			Settings settings = _settingsStore.Load();
			settings.EmailServer = host;
			_settingsStore.Save(settings);
		}

		[Action("set-email-port", ":port")]
		public void SetEmailPort(int port)
		{
			Settings settings = _settingsStore.Load();
			settings.EmailPort = port;
			_settingsStore.Save(settings);
		}

		[Action("set-email-secure", ":value")]
		public void SetEmailSecure(bool value)
		{
			Settings settings = _settingsStore.Load();
			settings.EmailSecure = value;
			_settingsStore.Save(settings);
		}

		[Action("set-email-from", ":address")]
		public void SetEmailFrom(string address)
		{
			Settings settings = _settingsStore.Load();
			settings.EmailFrom = address;
			_settingsStore.Save(settings);
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

