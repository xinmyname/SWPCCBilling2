using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class Parent
	{
		[Key]
		public long Id { get; set; }
		public long FamilyId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
	}
}

