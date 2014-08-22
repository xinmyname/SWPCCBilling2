using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Controllers
{
	public class FeeController : Controller
	{
		private readonly SettingsStore _settingsStore;
		private readonly FeeStore _feeStore;
		private readonly UrlFactory _urlFactory;

		public FeeController()
		{
			_settingsStore = new SettingsStore();
			_feeStore = new FeeStore();
			_urlFactory = UrlFactory.DefaultUrlFactory;
		}

		[Action("import-fees","path-to-csv-file")]
		public void ImportFees(
			[Optional]string path)
		{
			if (path == null)
			{
				Settings settings = _settingsStore.Load();
				path = settings.DefaultFeeImportPath;
			}

			var importer = new FeeImporter();

			_feeStore.RemoveAll();

			importer.FeeAction = _feeStore.Add;

			importer.ImportCsvAtPath(path);
		}

		[Action("show-fee","code")]
		public void ShowFee(
			[CompleteWith(typeof(FeeCompletion))] string code)
		{
			string url = _urlFactory.UrlForPath("/fee/{0}", code);

			Process.Start(url);
		}

		[Action("remove-fee","code")]
		public void RemoveFee(
			[CompleteWith(typeof(FeeCompletion))] string code)
		{
			_feeStore.Remove(code);
		}
	}
}

