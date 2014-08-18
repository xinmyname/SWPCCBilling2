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

		public InvoiceController()
		{
			_invoiceStore = new InvoiceStore();
			_familyStore = new FamilyStore();
		}

		[Action("open-invoice","family-name date(optional)")]
		public void OpenInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			DateTime invoiceDate = GetInvoiceDate(date);
			IList<string> familyNames = _familyStore.GetFamilyNames(name).ToList();

			foreach (string familyName in familyNames)
			{
				Family family = _familyStore.Load(familyName);
				Invoice invoice = _invoiceStore.Load(familyName, invoiceDate);

				if (invoice == null)
				{
					DateTime dueDate = new DateTime(invoiceDate.Year, invoiceDate.Month, family.DueDay);
					invoice = new Invoice(familyName, invoiceDate, dueDate);
					_invoiceStore.Add(invoice);

					Console.WriteLine("New invoice created for {0} on {1:d}",
						familyName, invoiceDate);
				}
				else
					Console.WriteLine("Invoice already exists for {0} on {1:d}",
						familyName, invoiceDate);
			}
		}

		[Action("close-invoice","family-name date(optional)")]
		public void CloseInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			throw new NotImplementedException();
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
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))] DateTime date)
		{
			throw new NotImplementedException();
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

