using System;
using SWPCCBilling2.Infrastructure;
using System.Runtime.InteropServices;
using SWPCCBilling2.Completions;
using System.Collections.Generic;
using System.Linq;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Controllers
{
	public class InvoiceController : Controller
	{
		private readonly InvoiceStore _invoiceStore;
		private readonly FamilyStore _familyStore;
		private readonly Ledger _ledger;

		public InvoiceController()
		{
			_invoiceStore = new InvoiceStore();
			_familyStore = new FamilyStore();
			_ledger = new Ledger();
		}

		[Action("open-invoice","family-name date(optional)")]
		public void OpenInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime invoiceDate = GetInvoiceDate(date);

			IList<Family> activeFamilies = _familyStore.LoadActiveFamilesWithName(name).ToList();

			foreach (Family family in activeFamilies)
			{
				IList<LedgerLine> ledgerLines = _ledger.LoadLinesWithoutInvoiceForFamily(family.Name);

				if (ledgerLines.Count == 0)
				{
					Console.WriteLine("No invoice created for {0} family - no account activity.", family.Name);
					continue;
				}

				Invoice invoice = _invoiceStore.Load(family.Name, invoiceDate);

				if (invoice == null)
				{
					DateTime dueDate = new DateTime(invoiceDate.Year, invoiceDate.Month, family.DueDay);
					invoice = new Invoice(family.Name, invoiceDate, dueDate);

					_invoiceStore.Add(invoice);

					Console.WriteLine("Creating invoice for {0} family on {1:d}", family.Name, invoiceDate);
				}
				else
					Console.WriteLine("Appending to existing opening invoice for {0} on {1:d}", family.Name, invoiceDate);

				foreach (LedgerLine ledgerLine in ledgerLines)
					invoice.AddLedgerLine(ledgerLine);

				Console.WriteLine("    Amount Due: {0:C}", invoice.AmountDue());

				_invoiceStore.Save(invoice);

				foreach (LedgerLine ledgerLine in ledgerLines)
				{
					ledgerLine.InvoiceId = invoice.Id;
					_ledger.SaveLine(ledgerLine);
				};
			}
		}

		[Action("close-invoice","family-name date(optional)")]
		public void CloseInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime invoiceDate = GetInvoiceDate(date);

			IList<Family> activeFamilies = _familyStore.LoadActiveFamilesWithName(name).ToList();

			foreach (Family family in activeFamilies)
			{
				Invoice invoice = _invoiceStore.Load(family.Name, invoiceDate);

				if (invoice == null)
				{
					Console.WriteLine("No invoice found for {0} family on {1:d}", family.Name, invoiceDate);
					continue;
				}

				if (invoice.AmountDue() != 0m)
				{
					Console.WriteLine("Invoice for {0} family on {1:d} cannot be closed because they owe {2:C}",
						family.Name, invoiceDate, invoice.AmountDue());

					continue;
				}

				invoice.Closed = DateTime.Now;
				_invoiceStore.Save(invoice, false);

				Console.WriteLine("Invoice {0} for {1} family on {2:d} has been closed.",
					invoice.Id, family.Name, invoiceDate);
			}
		}

		[Action("send-invoice","family-name date(optional)")]
		public void SendInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			throw new NotImplementedException();
		}

		[Action("remove-invoice","family-name date")]
		public void RemoveInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime invoiceDate = GetInvoiceDate(date);

			IList<Family> activeFamilies = _familyStore.LoadActiveFamilesWithName(name).ToList();

			foreach (Family family in activeFamilies)
			{
				Invoice invoice = _invoiceStore.Load(family.Name, invoiceDate);

				if (invoice == null)
				{
					Console.WriteLine("No invoice found for {0} family on {1:d}", family.Name, invoiceDate);
					continue;
				}

				_ledger.RemoveInvoiceId(invoice.Id);
				_invoiceStore.Remove(invoice);

				Console.WriteLine("Invoice {0} for {1} family on {2:d} has been removed.",
					invoice.Id, family.Name, invoiceDate);
			}
		}

		public DateTime GetInvoiceDate(DateTime? date)
		{
			DateTime now = DateTime.Now;
			DateTime nextMonth = now.AddMonths(1);

			DateTime invoiceDate = date != null 
				? date.Value
				: new DateTime(nextMonth.Year, nextMonth.Month, 1);

			return invoiceDate;
		}
	}
}

