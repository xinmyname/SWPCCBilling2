using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class ReportController : Controller
	{
		public ReportController()
		{
		}

		[Action("report-month","date")]
		public void ReportMonth(
			[CompleteWith(typeof(DateCompletion))] DateTime date)
		{
			throw new NotImplementedException();
		}

		[Action("report-unpaid","date")]
		public void ReportUnpaid(
			[CompleteWith(typeof(DateCompletion))] DateTime date)
		{
			throw new NotImplementedException();
		}

		[Action("report-payments","date")]
		public void ReportPayments(
			[CompleteWith(typeof(DateCompletion))] DateTime date)
		{
			throw new NotImplementedException();
		}
	}
}

