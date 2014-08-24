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

		public decimal SubTotal()
		{
			return ((decimal)UnitPrice * Quantity);
		}
	}
}
