using System;
using SWPCCBilling2.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Nancy;

namespace SWPCCBilling2.Infrastructure
{
	public class InvoiceViewModelFactory
	{
		private readonly InvoiceStore _invoiceStore;
		private readonly FamilyStore _familyStore;
		private readonly ParentStore _parentStore;
		private ILookup<string,string> _fees;

		public InvoiceViewModelFactory()
		{
			_invoiceStore = new InvoiceStore();
			_familyStore = new FamilyStore();
			_parentStore = new ParentStore();

			FeeStore feeStore = new FeeStore();

			_fees = feeStore.LoadAll().ToLookup(f => f.Code, f => f.Description);
		}

		public InvoiceViewModel Create(long invoiceId)
		{
			Invoice invoice = _invoiceStore.Load(invoiceId);
			Family family = _familyStore.Load(invoice.FamilyName);
			IList<Parent> parents = _parentStore.LoadForFamilyName(invoice.FamilyName);

			return new InvoiceViewModel 
			{
				Id = invoice.Id,
				Date = DateText(invoice.Opened, "MMM yyyy"),
				Family = family,
				Parents = parents,
				InvoiceLines = invoice.Lines.Select(ll => new InvoiceLineViewModel
				{
					Description = _fees[ll.FeeCode].FirstOrDefault(),
					Notes = ll.Notes,
					FeeCode = ll.FeeCode,
					UnitPrice = ll.UnitPrice.ToString("C"),
					Quantity = ll.Quantity,
					Amount = ll.Amount().ToString("C")

				}).ToList(),
				DueDate = DateText(invoice.Due, "MMMM d@@"),
				AmountDue = invoice.BalanceDue().ToString("C")
			};
		}

		public static string DateText(DateTime? date, string format)
		{
			if (date != null && date.HasValue)
			{
				string text = date.Value.ToString(format);

				if (text.IndexOf("@@") != -1)
				{
					switch (date.Value.Day)
					{
						case 1:
						case 21:
						case 31: 
							text = text.Replace("@@", "st"); 
							break;
						case 2: 
						case 22: 
							text = text.Replace("@@", "nd"); 
							break;
						case 3: 
						case 23: 
							text = text.Replace("@@", "rd"); 
							break;
						case 4: 
						case 5: 
						case 6: 
						case 7: 
						case 8: 
						case 9: 
						case 10: 
						case 11: 
						case 12: 
						case 13: 
						case 14: 
						case 15: 
						case 16: 
						case 17: 
						case 18: 
						case 19: 
						case 20: 
						case 24: 
						case 25: 
						case 26: 
						case 27: 
						case 28: 
						case 29: 
						case 30: 
							text = text.Replace("@@", "th"); 
							break;
					}
				}

				return text;
			}

			return String.Empty;
		}
	}
}
