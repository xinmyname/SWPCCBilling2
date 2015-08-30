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

		[Action("report-monthly","date")]
		public void ReportMonthly(
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime reportDate = _dateFactory.GetReportDate(date);

			string url = _urlFactory.UrlForPath("report/monthly/{0:yyyy-MM-dd}", reportDate);
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

		[Action("report-invoices","deposit")]
		public void ReportInvoices(
			string deposit)
		{
			string url = _urlFactory.UrlForPath("report/invoices/{0}", deposit.ToLower());
			Process.Start(url);
		}

		[Action("report-deposit")]
		public void ReportDeposit(
			[CompleteWith(typeof(DepositDateCompletion))]
			[Optional] long? depositId)
		{
			string deposit = (depositId == null)
				? "pending"
				: depositId.Value.ToString();

			string url = _urlFactory.UrlForPath("report/deposit/{0}", deposit);
			Process.Start(url);
		}
	}
}

