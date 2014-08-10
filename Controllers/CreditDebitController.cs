using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class CreditDebitController : Controller
	{
		public CreditDebitController()
		{
		}

		[Action("debit", ":familyName :feeName :quantity :amount")]
		public void Debit(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(FeeCompletion))] string feeName, 
			int quantity, 
			double amount)
		{
		}

		[Action("credit-fee", ":familyName :feeName :quantity :amount")]
		public void CreditFee(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(FeeCompletion))] string feeName, 
			int quantity, 
			double amount)
		{
		}

		[Action("credit-payment", ":familyName :checkNum :amount")]
		public void CreditPayment(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			string checkNum, 
			double amount)
		{
		}

		[Action("scan-payment")]
		public void ScanPayment()
		{
		}

		[Action("deposit-payment")]
		public void DepositPayment()
		{
		}

		[Action("show-payment")]
		public void ShowPayment()
		{
		}
	}
}

