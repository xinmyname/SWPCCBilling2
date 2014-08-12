using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;

namespace SWPCCBilling2.Controllers
{
	public class DiscountController : Controller
	{
		public DiscountController()
		{
		}

		[Action("import-discounts","path-to-csv-file")]
		public void ImportDiscounts(
			string path)
		{
			throw new NotImplementedException();
		}

		[Action("show-discount","discount-name")]
		public void ShowDiscount(
			[CompleteWith(typeof(DiscountCompletion))] string discountName)
		{
			throw new NotImplementedException();
		}

		[Action("remove-discount","discount-name")]
		public void RemoveDiscount(
			[CompleteWith(typeof(DiscountCompletion))] string discountName)
		{
			throw new NotImplementedException();
		}
	}
}

