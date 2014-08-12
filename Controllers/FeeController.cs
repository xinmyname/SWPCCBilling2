using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;

namespace SWPCCBilling2.Controllers
{
	public class FeeController : Controller
	{
		public FeeController()
		{
		}

		[Action("import-fees","path-to-csv-file")]
		public void ImportFees(
			string path)
		{
			var feeStore = new FeeStore();
			var importer = new FeeImporter();

			feeStore.RemoveAll();

			importer.FeeAction = feeStore.Add;

			importer.ImportCsvAtPath(path);
		}

		[Action("show-fee","fee-name")]
		public void ShowFee(
			[CompleteWith(typeof(FeeCompletion))] string feeName)
		{
			throw new NotImplementedException();
		}

		[Action("remove-fee","fee-name")]
		public void RemoveFee(
			[CompleteWith(typeof(FeeCompletion))] string feeName)
		{
			throw new NotImplementedException();
		}
	}
}

