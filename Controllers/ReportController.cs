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
		private readonly DateFactory _dateFactory;

		public ReportController()
		{
			_urlFactory = UrlFactory.DefaultUrlFactory;
			_dateFactory = DateFactory.DefaultDateFactory;
		}

		[Action("report-month","date")]
		public void ReportMonth(
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime reportDate = _dateFactory.GetReportDate(date);

			string url = _urlFactory.UrlForPath("report/month/{0:yyyy-MM-dd}", reportDate);
			Process.Start(url);
		}

		[Action("report-unpaid","date")]
		public void ReportUnpaid(
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime reportDate = _dateFactory.GetReportDate(date);

			string url = _urlFactory.UrlForPath("report/unpaid/{0:yyyy-MM-dd}", reportDate);
			Process.Start(url);
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

			string url = _urlFactory.UrlForPath("report/deposit/{0}", depositId);
			Process.Start(url);
		}
	}
}

