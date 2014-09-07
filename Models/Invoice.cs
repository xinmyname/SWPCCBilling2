using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SWPCCBilling2.Infrastructure;
using System.Linq;

namespace SWPCCBilling2.Models
{
	public class Invoice
	{
		[Key]
		public long Id { get; set; }
		public string FamilyName { get; set; }
		public DateTime? Due { get; set; }
		public DateTime? Sent { get; set; }
		public DateTime? Opened { get; set; }
		public DateTime? Closed { get; set; }

		[Ignore]
		public IList<InvoiceLine> Lines { get; set; }

		public Invoice()
		{
			Lines = new List<InvoiceLine>();
		}

		public Invoice(string familyName, DateTime opened, DateTime due)
			: this()
		{
			FamilyName = familyName;
			Due = due;
			Opened = opened;
		}

		public decimal AmountDue()
		{
			decimal amountDue = 0;

			foreach (InvoiceLine line in Lines.Where(l => l.FeeCode != "Payment"))
				amountDue += line.Amount();

			return amountDue;
		}

		public decimal BalanceDue() 
		{
			decimal balance = 0;

			foreach (InvoiceLine line in Lines)
				balance += line.Amount();

			return balance;
		}

		public void AddLedgerLine(LedgerLine ledgerLine)
		{
			Lines.Add(new InvoiceLine 
			{
				InvoiceId = this.Id,
				FeeCode = ledgerLine.FeeCode,
				Quantity = ledgerLine.Quantity,
				UnitPrice = ledgerLine.UnitPrice,
				Notes = ledgerLine.Notes
			});
		}
	}

	public class InvoiceLine
	{
		public long InvoiceId { get; set; }
		public string FeeCode { get; set; }
		public long Quantity { get; set; }
		public double UnitPrice { get; set; }
		public string Notes { get; set; }

		public decimal Amount()
		{
			return (decimal)UnitPrice * (decimal)Quantity;
		}
	}
}

