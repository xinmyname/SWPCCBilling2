﻿using System;
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
	}

	public class InvoiceLine
	{
		public long InvoiceId { get; set; }
		public string FeeCode { get; set; }
		public double UnitPrice { get; set; }
		public long Quantity { get; set; }
		public string Notes { get; set; }
	}
}
