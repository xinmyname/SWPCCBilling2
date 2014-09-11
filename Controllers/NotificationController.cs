using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Controllers
{
	public class NotificationController : Controller
	{
		private readonly Mailer _mailer;
		private readonly InvoiceStore _invoiceStore;
		private readonly DateFactory _dateFactory;
		private readonly FamilyStore _familyStore;
		private readonly ParentStore _parentStore;

		public NotificationController()
		{
			_mailer = new Mailer();
			_invoiceStore = new InvoiceStore();
			_dateFactory = DateFactory.DefaultDateFactory;
			_familyStore = new FamilyStore();
			_parentStore = new ParentStore();
		}

		[Action("send-2day-warning","family-name(optional)")]
		public void SendTwoDayWarning(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			Console.Write("Password? ");
			string password = Console.ReadLine();

			DateTime now = _dateFactory.Now();
			DateTime reportDate = _dateFactory.GetReportDate(null);
			IList<Invoice> openInvoices = _invoiceStore.LoadOpenInvoicesAfter(reportDate);
			IList<string> overdueAddresses = openInvoices
				.Select(i => _familyStore.Load(i.FamilyName))
				.Where(f => now.Day >= f.DueDay)
				.SelectMany(f => _parentStore.LoadForFamilyName(f.Name))
				.Select(p => p.Email)
				.Where(e => !String.IsNullOrEmpty(e))
				.ToList();

			string monthText = reportDate.ToString("MMMMM yyyy");
			DateTime graceDate = reportDate.AddDays(11);

			if (graceDate.DayOfWeek == DayOfWeek.Sunday)
				graceDate = graceDate.AddDays(1);

			string graceDateText = graceDate.ToString("dddd, MMMMM ") + _dateFactory.CardinalDay(graceDate);

			string body = String.Format("<html><body style=\"font-family: font-family: 'Calibri', 'Helvetica Neue', 'Arial', sans-serif;\"><p>We haven't received your payment for {0} yet. Payments received after {1} may be subject to a " +
				"$5 late fee. If you are going to mail your payment, please make sure it's postmarked on or before {1}.</p>\n\n<p>If your check " +
				"is already in the mail, please disregard this message.</p>\n\n<p>If you have any questions or concerns, please don't hesitate to email me.</p></body></html>", 
				monthText, graceDateText);

			string subject = String.Format("SWPCC {0} Tuition Not Received", monthText);

			_mailer.SendSecretly(subject, body, password, overdueAddresses);
		}

		[Action("send-2day-violation","family-name(optional)")]
		public void SendTwoDayViolation(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("send-7day-warning","family-name(optional)")]
		public void SendSevenDayWarning(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("send-7day-violation","family-name(optional)")]
		public void SendSevenDayViolation(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}
	}
	
}
