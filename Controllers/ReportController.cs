using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SWPCCBilling2.Controllers
{
	public class ReportController : Controller
	{
		private readonly UrlFactory _urlFactory;

		public ReportController()
		{
			_urlFactory = UrlFactory.DefaultUrlFactory;
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

		[Action("report-deposit")]
		public void ReportDeposit(
			[CompleteWith(typeof(DepositDateCompletion))][Optional] DateTime? depositDate)
		{
			string depositId = "pending";

			if (depositDate != null)
			{
				throw new NotImplementedException("Need to get deposit ID");
			}

			// Move existing report to deposit pending report
			string url = _urlFactory.UrlForPath("report/deposit/{0}", depositId);

			Process.Start(url);
		}
	}
}

