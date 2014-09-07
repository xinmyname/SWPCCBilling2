using System;
using Nancy;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Modules
{
	public class ReportModule : NancyModule
	{
		public ReportModule(PaymentStore paymentStore, InvoiceStore invoiceStore, ParentStore parentStore)
		{
			Get["/report/deposit/pending"] = _ =>
			{
				IList<Payment> undeposited = paymentStore.LoadUndeposited().ToList();
				decimal depositAmount = undeposited.Sum(x => (decimal)x.Amount);

				return View["DepositPending", new {
					Checks = undeposited.Select(p => new {
						p.FamilyName,
						p.CheckNum,
						p.Amount,
						AmountText = p.Amount.ToString("C")
					}),
					Amount = depositAmount,
					AmountText = depositAmount.ToString("C")
				}];
			};

			Get["/report/deposit/{id}"] = _ =>
			{
				throw new NotImplementedException();
			};

			Get["/report/unpaid/{date}"] = _ =>
			{
				IList<Invoice> unpaidInvoices = invoiceStore.LoadOpenInvoicesAfter(_.date);


				IEnumerable<string> unpaidEmails = unpaidInvoices
					.SelectMany(i => 
						parentStore
							.LoadForFamilyName(i.FamilyName)
							.Select(p => p.Email)
							.Where(e => e != null));

				return View["Unpaid", unpaidEmails];
			};

			Get["/report/monthly/{date}"] = _ =>
			{
				DateTime month = _.date;
				IList<Invoice> monthlyInvoices = invoiceStore.LoadInvoicesForMonth(month);
				var model = new MonthlyData();

				model.Month = month.ToString("MMMM yyyy");
				model.InvoiceSummaries = monthlyInvoices
					.OrderBy(i => i.Closed)
					.Select(i => new MonthlyInvoiceSummary(i)).ToList();
				model.TotalDue = monthlyInvoices.Sum(i => i.AmountDue()).ToString("C");

				foreach (Invoice invoice in monthlyInvoices)
				{
				}

				return View["Monthly", model];
			};
		}
	}

	public class MonthlyData
	{
		public string Month { get; set; }
		public IList<MonthlyInvoiceSummary> InvoiceSummaries { get; set; }
		public string TotalDue { get; set; }
		public string TotalPaid { get; set; }
		public string TotalDonated { get; set; }
		public IList<string> DepositHeaderHtml { get; set; }
		public IList<string> DepositRowHtml { get; set; }

		public MonthlyData()
		{
			InvoiceSummaries = new List<MonthlyInvoiceSummary>();
			DepositHeaderHtml = new List<string>();
			DepositRowHtml = new List<string>();
		}
	}

	public class MonthlyInvoiceSummary
	{
		public string FamilyName { get; set; }
		public string Due { get; set; }
		public string Paid { get; set; }
		public string Donated { get; set; }
		public string CheckNumbers { get; set; }
		public string Closed { get; set; }

		public MonthlyInvoiceSummary(Invoice invoice)
		{
			FamilyName = invoice.FamilyName;
			Due = invoice.AmountDue().ToString("C");

			if (invoice.Closed != null)
				Closed = invoice.Closed.Value.ToString("d");
		}
	}
}
