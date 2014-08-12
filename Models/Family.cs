using System;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Models
{
	public class Family
	{
		[Key]
		public long Id { get; set; }
		public string Name { get; set; }
		public string StreetAddress { get; set; }
		public string City { get; set;}
		public string State { get; set; }
		public string Zip { get; set; }
		public int DueDay { get; set; }
		public int NumChildren { get; set; }
		public int BillableDays { get; set; }
        public bool IsNew { get; set; }
		public bool IsGraduating { get; set; }
		public string CheckSHA256 { get; set; }
		public DateTime? Joined { get; set; }
		public DateTime? Departed { get; set; }
	}
}

