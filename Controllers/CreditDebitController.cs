using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;

namespace SWPCCBilling2.Controllers
{
	public class CreditDebitController : Controller
	{
		public CreditDebitController()
		{
		}

		[Action("debit", "family-name fee-name quantity(number) amount(dollars)")]
		public void Debit(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(FeeCompletion))] string feeName, 
			int quantity, 
			double amount)
		{
			throw new NotImplementedException();
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

