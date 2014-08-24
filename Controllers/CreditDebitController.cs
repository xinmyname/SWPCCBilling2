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
		private readonly Ledger _ledger;
		private readonly FamilyStore _familyStore;
		private readonly FeeStore _feeStore;

		public CreditDebitController()
		{
			_ledger = new Ledger();
			_familyStore = new FamilyStore();
			_feeStore = new FeeStore();
		}

		[Action("debit", "family-name fee-name quantity(number) amount(dollars)")]
		public void Debit(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(FeeCompletion))] string feeCode, 
			[Optional]long? quantity, 
			[Optional]double? amount)
		{
			IList<Family> activeFamilies = _familyStore.LoadActiveFamilesWithName(name).ToList();
			Fee fee = _feeStore.Load(feeCode);

			decimal total = 0m;

			foreach (Family family in activeFamilies)
				total += _ledger.Debit(family, fee, quantity, amount, null).SubTotal();

			Console.WriteLine("Debited {0} families {1:C}", activeFamilies.Count, total);
		}

		[Action("credit-fee", "family-name fee-name quantity(number) amount(dollars)")]
		public void CreditFee(
			[CompleteWith(typeof(FamilyCompletion))] string name, 
			[CompleteWith(typeof(FeeCompletion))] string feeCode, 
			[Optional]long? quantity, 
			[Optional]double? amount)
		{
			IList<Family> activeFamilies = _familyStore.LoadActiveFamilesWithName(name).ToList();
			Fee fee = _feeStore.Load(feeCode);

			decimal total = 0m;

			foreach (Family family in activeFamilies)
				total += _ledger.Credit(family, fee, quantity, amount, null).SubTotal();

			Console.WriteLine("Credited {0} families {1:C}", activeFamilies.Count, -total);
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

