using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
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
			throw new NotImplementedException();
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

