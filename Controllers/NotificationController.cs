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

			DateTime reportDate = _dateFactory.GetReportDate(null);
			string monthText = reportDate.ToString("MMMMM yyyy");
			string subject = String.Format("SWPCC {0} Tuition Not Received", monthText);

			foreach (Family family in OverdueFamilies(familyName).ToList())
			{
				IList<string> emails = ParentEmails(family).ToList();
				DateTime grace1Date = _dateFactory.Grace1Date(family.DueDay);

				string body = String.Format("<html><body style=\"font-family: font-family: 'Calibri', 'Helvetica Neue', 'Arial', sans-serif;\"><p>We haven't received your payment for {0} yet. Payments received after {1} may be subject to a " +
					"$5 late fee. If you are going to mail your payment, please make sure it's postmarked on or before {1}.</p>\n\n<p>If your check " +
					"is already in the mail, please disregard this message.</p>\n\n<p>If you have any questions or concerns, please don't hesitate to email me.</p></body></html>", 
					monthText, grace1Date.ToCardinalDateString());

				_mailer.Send(subject, body, password, emails);
			}
		}

		[Action("send-2day-violation","family-name(optional)")]
		public void SendTwoDayViolation(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			Console.Write("Password? ");
			string password = Console.ReadLine();

			DateTime reportDate = _dateFactory.GetReportDate(null);
			string monthText = reportDate.ToString("MMMMM yyyy");
			string subject = String.Format("SWPCC {0} Tuition Not Received - 2nd notice", monthText);

			foreach (Family family in OverdueFamilies(familyName).ToList())
			{
				IList<string> emails = ParentEmails(family).ToList();
				DateTime grace1Date = _dateFactory.Grace1Date(family.DueDay);
				DateTime grace2Date = _dateFactory.Grace2Date(family.DueDay);

				string body = String.Format("<html><body style=\"font-family: font-family: 'Calibri', 'Helvetica Neue', 'Arial', sans-serif;\">" +
					"<p>We have not received your payment for {0}. A $5 late fee has been applied to your account and " +
					"will appear on your next invoice. Payments received after {1} may be subject to an additional $20 late fee. </p>\n\n" +
					"<p>If you've dropped your check in the tuition box or mailed it on or before {2}, " +
					"please let me know and I will rescind the late fee.</p></body></html>", 
					monthText, grace2Date.ToCardinalDateString(), grace1Date.ToCardinalDateString());

				_mailer.Send(subject, body, password, emails);
			}
		}

		[Action("send-7day-violation","family-name(optional)")]
		public void SendSevenDayViolation(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			Console.Write("Password? ");
			string password = Console.ReadLine();

			DateTime reportDate = _dateFactory.GetReportDate(null);
			string monthText = reportDate.ToString("MMMMM yyyy");
			string subject = String.Format("SWPCC {0} Tuition Not Received - 2nd notice", monthText);

			foreach (Family family in OverdueFamilies(familyName).ToList())
			{
				IList<string> emails = ParentEmails(family).ToList();
				DateTime grace2Date = _dateFactory.Grace2Date(family.DueDay);

				string body = String.Format("<html><body style=\"font-family: font-family: 'Calibri', 'Helvetica Neue', 'Arial', sans-serif;\">" +
					"<p>We have not received your payment for {0}. An additional $20 late fee has been applied to your account and " +
					"will appear on your next invoice.</p>\n\n" +
					"<p>If you've dropped your check in the tuition box or mailed it on or before {1}, " +
					"please let me know and I will rescind the late fee.</p></body></html>", 
					monthText, grace2Date.ToCardinalDateString());

				_mailer.Send(subject, body, password, emails);
			}
		}

		private IEnumerable<Family> OverdueFamilies(string familyName)
		{
			if (familyName != null)
			{
				Family family = _familyStore.Load(familyName);

				if (family == null)
					throw new Error("{0} family not in database.", familyName);

				yield return family;
			}
			else
			{
				DateTime now = _dateFactory.Now();
				DateTime reportDate = _dateFactory.GetReportDate(null);
				IList<Invoice> openInvoices = _invoiceStore.LoadOpenInvoicesAfter(reportDate);
				IEnumerable<Family> overdueFamilies = openInvoices
					.Select(i => _familyStore.Load(i.FamilyName))
					.Where(f => now.Day >= f.DueDay);

				foreach (Family family in overdueFamilies)
					yield return family;
			}
		}

		private IEnumerable<string> ParentEmails(Family family)
		{
			return _parentStore.LoadForFamilyName(family.Name)
				.Select(p => p.Email)
				.Where(e => !String.IsNullOrEmpty(e));
		}
	}
	
}
