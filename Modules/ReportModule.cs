using System;
using Nancy;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPCCBilling2.Modules
{
	public class ReportModule : NancyModule
	{
		public ReportModule(PaymentStore paymentStore, InvoiceStore invoiceStore, ParentStore parentStore, DepositStore depositStore)
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
					.Select(i => new MonthlyInvoiceSummary(i, paymentStore.LoadPaymentsForInvoice(i)))
					.ToList();

				model.TotalDue = model.InvoiceSummaries.Sum(mis => mis.Due);
				model.TotalDueText = model.TotalDue.ToHtmlCurrency();
				model.TotalCreditUsed = model.InvoiceSummaries.Sum(mis => mis.CreditUsed);
				model.TotalCreditUsedText = model.TotalCreditUsed.ToHtmlCurrency();
				model.TotalPaid = model.InvoiceSummaries.Sum(mis => mis.Paid);
				model.TotalPaidText = model.TotalPaid.ToHtmlCurrency();
				model.TotalCreditDue = model.InvoiceSummaries.Sum(mis => mis.CreditDue);
				model.TotalCreditDueText = model.TotalCreditDue.ToHtmlCurrency();
				model.TotalDonated = model.InvoiceSummaries.Sum(mis => mis.Donated);
				model.TotalDonatedText = model.TotalDonated.ToHtmlCurrency();

				IDictionary<long, IList<long>> invoicesForDeposits;

				invoicesForDeposits = depositStore.InvoicesForDeposits(month);

				foreach (long depositId in invoicesForDeposits.Keys)
				{
					Deposit deposit = depositStore.Load(depositId);
					IList<long> invoiceIds = invoicesForDeposits[depositId];
					IDictionary<string, double> categoryTotals = invoiceStore.CategoryTotals(invoiceIds);

					// TODO: Build matrix from deposit and category totals
					throw new NotImplementedException();
				}

				return View["Monthly", model];
			};
		}
	}

	public class MonthlyData
	{
		public string Month { get; set; }
		public IList<MonthlyInvoiceSummary> InvoiceSummaries { get; set; }
		public decimal TotalDue { get; set; }
		public decimal TotalCreditUsed { get; set; }
		public decimal TotalPaid { get; set; }
		public decimal TotalCreditDue { get; set; }
		public decimal TotalDonated { get; set; }
		public string TotalDueText { get; set; }
		public string TotalCreditUsedText { get; set; }
		public string TotalPaidText { get; set; }
		public string TotalCreditDueText { get; set; }
		public string TotalDonatedText { get; set; }
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

		public decimal Due { get; set; }
		public decimal CreditUsed { get; set; }
		public decimal Paid { get; set; }
		public decimal CreditDue { get; set; }
		public decimal Donated { get; set; }

		public string DueText { get; set; }
		public string CreditUsedText { get; set; }
		public string PaidText { get; set; }
		public string CreditDueText { get; set; }
		public string DonatedText { get; set; }

		public string CheckNumbers { get; set; }
		public string Closed { get; set; }

		public MonthlyInvoiceSummary(Invoice invoice, IEnumerable<Payment> payments)
		{
			FamilyName = invoice.FamilyName;
			Due = AmountDue(invoice);
			DueText = Due.ToHtmlCurrency();
			CreditUsed = AmountCreditUsed(invoice);
			CreditUsedText = CreditUsed.ToHtmlCurrency();
			Paid = -AmountPaid(invoice);
			PaidText = Paid.ToHtmlCurrency();
			CreditDue = AmountCreditDue(invoice);
			CreditDueText = CreditDue.ToHtmlCurrency();
			Donated = AmountDonated(invoice);
			DonatedText = Donated.ToHtmlCurrency();

			var checkNumbers = new StringBuilder();

			foreach (Payment payment in payments)
			{
				if (checkNumbers.Length > 0)
					checkNumbers.Append(',');

				checkNumbers.Append(payment.CheckNum);
			}

			CheckNumbers = checkNumbers.ToString();

			if (invoice.Closed != null)
				Closed = invoice.Closed.Value.ToString("d");
		}

		private decimal AmountDue(Invoice invoice)
		{
			decimal amount = 0;

			foreach (InvoiceLine line in invoice.Lines.Where(l => l.FeeCode != "Payment" && l.FeeCode != "CreditNext"))
				amount += line.Amount();

			return amount;
		}

		private decimal AmountCreditUsed(Invoice invoice)
		{
			decimal amount = 0;

			foreach (InvoiceLine line in invoice.Lines.Where(l => l.FeeCode == "CreditPrev"))
				amount += line.Amount();

			return amount;
		}

		private decimal AmountPaid(Invoice invoice)
		{
			decimal amount = 0;

			foreach (InvoiceLine line in invoice.Lines.Where(l => l.FeeCode == "Payment"))
				amount += line.Amount();

			return amount;
		}

		private decimal AmountCreditDue(Invoice invoice)
		{
			decimal amount = 0;

			foreach (InvoiceLine line in invoice.Lines.Where(l => l.FeeCode == "CreditNext"))
				amount += line.Amount();

			return amount;
		}

		private decimal AmountDonated(Invoice invoice)
		{
			decimal amount = 0;

			foreach (InvoiceLine line in invoice.Lines.Where(l => l.FeeCode == "Donation"))
				amount += line.Amount();

			return amount;
		}
	}
}
