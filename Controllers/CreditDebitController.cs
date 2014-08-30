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
		private readonly PaymentStore _paymentStore;
		private readonly InvoiceStore _invoiceStore;

		public CreditDebitController()
		{
			_ledger = new Ledger();
			_familyStore = new FamilyStore();
			_feeStore = new FeeStore();
			_paymentStore = new PaymentStore();
			_invoiceStore = new InvoiceStore();
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
				total += _ledger.Debit(family, null, fee, quantity, amount, null).SubTotal();

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
				total += _ledger.Credit(family, null, fee, quantity, amount, null).SubTotal();

			Console.WriteLine("Credited {0} families {1:C}", activeFamilies.Count, -total);
		}

		[Action("credit-payment", "family-name check-num amount")]
		public void CreditPayment(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			string checkNum, 
			double amount)
		{
			Family family = _familyStore.Load(familyName);

			if (family == null)
				throw new Error("{0} family not in database.", familyName);

			Invoice invoice = _invoiceStore.LoadLatestOpenInvoiceForFamily(familyName);

			if (invoice == null)
				throw new Error("There is no open invoice for the {0} family.", familyName);

			Payment payment = new Payment 
			{
				FamilyName = familyName,
				CheckNum = checkNum,
				Amount = amount,
				Received = DateTime.Now,
				InvoiceId = invoice.Id,
				DepositId = null
			};

			ApplyPayment(payment, invoice, family);
		}

		[Action("scan-payment")]
		public void ScanPayment()
		{
			while (true)
			{
				Console.WriteLine("Insert check into scanner or press Enter to stop...");
				string micrText = Console.ReadLine().Trim();

				if (String.IsNullOrEmpty(micrText))
					break;

				var micr = new MICR(micrText);

				IList<Family> families = _familyStore.LoadWithMICR(micr).ToList();

				if (families.Count == 0)
					throw new Error("No matching family found for check #", micr.CheckNumber);

				Family family = PickFamily(families);

				Console.Write("Check amount? ");
				string amountText = Console.ReadLine();
				double amount = Double.Parse(amountText);

				CreditPayment(family.Name, micr.CheckNumber, amount);
			}
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

		private void ApplyPayment(Payment payment, Invoice invoice, Family family)
		{
			Fee paymentFee = _feeStore.Load("Payment");
			bool paymentAdded = false;
			LedgerLine ledgerLine = null;

			try
			{
				_paymentStore.Add(payment);
				paymentAdded = true;

				ledgerLine = _ledger.Credit(family, invoice, paymentFee, 1, payment.Amount, null);

				invoice.AddLedgerLine(ledgerLine);

				decimal amountDue = invoice.AmountDue();

				if (amountDue < 0)
				{
					Console.WriteLine("The {0} family overpaid by {1:C}. A credit will be applied to their next invoice.",
						family.Name, -amountDue);

					Fee creditNextFee = _feeStore.Load("CreditNext");
					Fee creditPrevFee = _feeStore.Load("CreditPrev");

					var amount = (double)-amountDue;

					LedgerLine debitLine = _ledger.Debit(family, invoice, creditNextFee, 1, amount, null);

					_ledger.Credit(family, null, creditPrevFee, 1, amount, null);

					invoice.AddLedgerLine(debitLine);

					amountDue = 0;
				}
				else if (amountDue > 0)
				{
					Console.WriteLine("The {0} family still owes {1:C} on invoice {2}",
						family.Name, amountDue, invoice.Id);
				}

				// NOTE: Don't try to be cute and move this up to the if/else above. Invoices with a credit need to be closed.
				if (amountDue == 0)
				{
					Console.WriteLine("Invoice {0} paid in full and marked as closed.", invoice.Id);
					invoice.Closed = DateTime.Now;
				}

				_invoiceStore.Save(invoice, true);
			}
			catch (Exception ex)
			{
				if (ledgerLine != null)
					_ledger.RemoveLine(ledgerLine);

				if (paymentAdded)
					_paymentStore.Remove(payment);

				throw new Error("There was an error applying the payment.\n" +
					"The ledger entry was{0} removed.\n" +
					"The payment was{1} removed.\n\nDetails:{2}", 
					paymentAdded ? "" : " not",
					ledgerLine != null ? "" : " not",
					ex);
			}
		}

		private Family PickFamily(IList<Family> families)
		{
			if (families.Count == 1)
			{
				Family family = families.Single();
				Console.WriteLine("Matched check to {0} family", family.Name);
				return family;
			}

			Console.WriteLine("There are {0} families that match.\n", families.Count);

			for (int i = 0; i < families.Count; i++)
				Console.WriteLine("{0}) {1}", i + 1, families[i].Name);

			Console.WriteLine("\nWhich family should be used? ");

			string familyChoice = Console.ReadLine();
			int idx = Int32.Parse(familyChoice) - 1;

			if (idx < 0 || idx >= families.Count)
				throw new Error("That wasn't a choice.");

			return families[idx];
		}
	}
}

