using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class Payment
	{
		[Key]
		public long Id { get; set; }
		public string FamilyName { get; set; }
		public string CheckNum { get; set; }
		public double Amount { get; set; }
		public DateTime Received { get; set; }
		public long InvoiceId { get; set; }
		public long? DepositId  { get; set; }
	}
}
