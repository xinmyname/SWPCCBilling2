using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using System.Diagnostics;

namespace SWPCCBilling2.Controllers
{
	public class FeeController : Controller
	{
		private readonly FeeStore _feeStore;
		private readonly UrlFactory _urlFactory;

		public FeeController()
		{
			_feeStore = new FeeStore();
			_urlFactory = UrlFactory.DefaultUrlFactory;
		}

		[Action("import-fees","path-to-csv-file")]
		public void ImportFees(
			string path)
		{
			var importer = new FeeImporter();

			_feeStore.RemoveAll();

			importer.FeeAction = _feeStore.Add;

			importer.ImportCsvAtPath(path);
		}

		[Action("show-fee","code")]
		public void ShowFee(
			[CompleteWith(typeof(FeeCompletion))] string code)
		{
			long id = _feeStore.GetIdForCode(code);
			string url = _urlFactory.UrlForPath("/fee/{0}", id);

			Process.Start(url);
		}

		[Action("remove-fee","code")]
		public void RemoveFee(
			[CompleteWith(typeof(FeeCompletion))] string code)
		{
			long id = _feeStore.GetIdForCode(code);
			_feeStore.Remove(id);
		}
	}
}

