using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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

		public void AddLine(Family family, Fee fee, int? qty, double? amt)
		{
			double unitPrice = 0.0;
			long quantity = 0;

			switch (fee.Type)
			{
				case Fee.FeeTypeFixed:
				case Fee.FeeTypePerMinute:
					unitPrice = fee.Amount;
					quantity = qty ?? 1;
					break;
				case Fee.FeeTypeVarying:
					unitPrice = amt ?? 0.0;
					quantity = qty ?? 1;
					break;
				case Fee.FeeTypePerChild:
					unitPrice = fee.Amount;
					quantity = family.NumChildren;
					break;
				case Fee.FeeTypePerChildDay:
					unitPrice = fee.Amount;
					quantity = family.BillableDays;
					break;
			}

			Lines.Add(new InvoiceLine 
			{
				InvoiceId = Id,
				FeeCode = fee.Code,
				Quantity = quantity,
				UnitPrice = unitPrice
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
	}
}

