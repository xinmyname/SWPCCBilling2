using System;
using SWPCCBilling2.Models;
using System.Collections.Generic;

namespace SWPCCBilling2
{
	public class InvoiceViewModel
	{
		public long Id { get; set; } 
		public string Date { get; set; }
		public Family Family { get; set; }
		public IList<Parent> Parents { get; set; }
		public IList<InvoiceLineViewModel> InvoiceLines { get; set; }
		public string DueDate { get; set; }
		public string AmountDue { get; set; }
	}

	public class InvoiceLineViewModel
	{
		public string Description { get; set; }
		public string Notes { get; set; }
		public string FeeCode { get; set; }
		public string UnitPrice { get; set; }
		public long Quantity { get; set; }
		public string Amount { get; set; }
	}
}
