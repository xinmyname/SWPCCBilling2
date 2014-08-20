using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using SWPCCBilling2.Models;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Controllers
{
	public class CreditDebitController : Controller
	{
		private readonly InvoiceStore _invoiceStore;
		private readonly FamilyStore _familyStore;
		private readonly FeeStore _feeStore;

		public CreditDebitController()
		{
			_invoiceStore = new InvoiceStore();
			_familyStore = new FamilyStore();
			_feeStore = new FeeStore();
		}

		[Action("debit", "family-name fee-name quantity(number) amount(dollars)")]
		public void Debit(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(FeeCompletion))] string feeCode, 
			[Optional]int? quantity, 
			[Optional]double? amount)
		{
			Fee fee = _feeStore.Load(feeCode);

			foreach (Family family in _familyStore.LoadActiveFamilesWithName(name))
			{
				string familyName = family.Name;
				Invoice invoice = _invoiceStore.LoadLatestOpenInvoiceForFamily(familyName);

				if (invoice == null)
				{
					Console.WriteLine("Could not find an open invoice for {0} family.", familyName);
					continue;
				}

				invoice.AddLine(family, fee, quantity, amount);

				_invoiceStore.Save(invoice);
			}
		}

		[Action("credit-fee", "family-name fee-name quantity(number) amount(dollars)")]
		public void CreditFee(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(FeeCompletion))] string feeName, 
			int quantity, 
			double amount)
		{
			throw new NotImplementedException();
		}

		[Action("credit-payment", "family-name check-num amount")]
		public void CreditPayment(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			string checkNum, 
			double amount)
		{
			throw new NotImplementedException();
		}

		[Action("scan-payment")]
		public void ScanPayment()
		{
			throw new NotImplementedException();
		}

		[Action("deposit-payment")]
		public void DepositPayment()
		{
			throw new NotImplementedException();
		}

		[Action("show-payment")]
		public void ShowPayment()
		{
			throw new NotImplementedException();
		}
	}
}

