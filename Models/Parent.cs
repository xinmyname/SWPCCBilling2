using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class Parent
	{
		public string FamilyName { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }

		public override string ToString()
		{
			return String.Format("{0}\t{1}\t{2}", FamilyName, Name, Email);
		}
	}
}

