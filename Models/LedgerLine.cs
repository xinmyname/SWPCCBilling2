using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class LedgerLine
	{
		[Key]
		public long Id { get; set; }
		public string FamilyName { get; set; }
		public DateTime Date { get; set; }
		public long? InvoiceId { get; set; }
		public long? PaymentId { get; set; }
		public string FeeCode { get; set; }
		public long Quantity { get; set; }
		public double UnitPrice { get; set; }
		public string Notes { get; set; }

		public LedgerLine()
		{
		}

		public LedgerLine(Family family, Fee fee, long? quantity, double? amount, bool isCredit)
		{
			FamilyName = family.Name;
			FeeCode = fee.Code;
			Date = DateFactory.DefaultDateFactory.Now();

			if (quantity != null && amount != null)
			{
				UnitPrice = amount.Value;
				Quantity = quantity.Value;
			}
			else
			{
				switch (fee.Type)
				{
					case Fee.FeeTypeFixed:
					case Fee.FeeTypePerMinute:
						UnitPrice = fee.Amount;
						Quantity = quantity ?? 1;
						break;
					case Fee.FeeTypeVarying:
						UnitPrice = amount ?? 0.0;
						Quantity = quantity ?? 1;
						break;
					case Fee.FeeTypePerChild:
						UnitPrice = fee.Amount;
						Quantity = family.NumChildren;
						break;
					case Fee.FeeTypePerChildDay:
						UnitPrice = fee.Amount;
						Quantity = family.BillableDays;
						break;
				}
			}

			if (isCredit)
				UnitPrice = -UnitPrice;
		}

		public decimal SubTotal()
		{
			return ((decimal)UnitPrice * Quantity);
		}
	}
}
