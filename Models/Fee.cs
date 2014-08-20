using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class Fee
	{
		[Key]
		public string Code { get; set; }
		public string Description { get; set; }
		public string Type { get; set;}
		public string Category { get; set; }
		public double Amount { get; set; }

		public const string FeeTypeFixed = "F";
		public const string FeeTypeVarying = "V";
		public const string FeeTypePerChild = "C";
		public const string FeeTypePerChildDay = "D";
		public const string FeeTypePerMinute = "M";
	}
}

