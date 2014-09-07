using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using SWPCCBilling2.Models;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPCCBilling2.Controllers
{
	public class CreditDebitController : Controller
	{
		private readonly Ledger _ledger;
		private readonly FamilyStore _familyStore;
		private readonly FeeStore _feeStore;
		private readonly PaymentStore _paymentStore;
		private readonly InvoiceStore _invoiceStore;
		private readonly DateFactory _dateFactory;

		public CreditDebitController()
		{
			_ledger = new Ledger();
			_familyStore = new FamilyStore();
			_feeStore = new FeeStore();
			_paymentStore = new PaymentStore();
			_invoiceStore = new InvoiceStore();
			_dateFactory = DateFactory.DefaultDateFactory;
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
				total += _ledger.Debit(family, fee, quantity, amount).SubTotal();

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
				total += _ledger.Credit(family, fee, quantity, amount).SubTotal();

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
				Received = _dateFactory.Now(),
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
				Invoice invoice = _invoiceStore.LoadLatestOpenInvoiceForFamily(family.Name);

				Console.Write("Check amount? ");

				int curLeft = Console.CursorLeft;
				int curTop = Console.CursorTop;

				Console.Write("{0:C}", invoice.BalanceDue());

				ConsoleKeyInfo keyInfo = Console.ReadKey();

				decimal amount = 0.0m;

				if (keyInfo.Key == ConsoleKey.Enter)
					amount = invoice.BalanceDue();
				else
				{
					Console.SetCursorPosition(curLeft, curTop);
					Console.Write("            ");
					Console.SetCursorPosition(curLeft, curTop);
					Console.Write(keyInfo.KeyChar);

					var amountText = new StringBuilder();

					amountText.Append(keyInfo.KeyChar);
					amountText.Append(Console.ReadLine());

					amount = Decimal.Parse(amountText.ToString());
				}

				CreditPayment(family.Name, micr.CheckNumber, (double)amount);
			}
		}

		[Action("deposit")]
		public void Deposit()
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

				ledgerLine = _ledger.Credit(family, paymentFee, invoice, payment);

				invoice.AddLedgerLine(ledgerLine);

				decimal amountDue = invoice.BalanceDue();

				if (amountDue < 0)
				{
					amountDue = -amountDue;

					Console.WriteLine("The {0} family overpaid by {1:C}. A credit will be applied to their next invoice.",
						family.Name, amountDue);

					Fee creditNextFee = _feeStore.Load("CreditNext");
					Fee creditPrevFee = _feeStore.Load("CreditPrev");

					LedgerLine debitLine = _ledger.Debit(family, creditNextFee, invoice, amountDue);

					invoice.AddLedgerLine(debitLine);

					_ledger.Credit(family, creditPrevFee, amountDue);

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
					invoice.Closed = _dateFactory.Now();
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

