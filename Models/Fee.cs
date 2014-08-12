using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class Fee
	{
		[Key]
		public long Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public string Type { get; set;}
		public string Category { get; set; }
		public double Amount { get; set; }
	}
}

