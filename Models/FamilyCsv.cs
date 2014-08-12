using System;

namespace SWPCCBilling2
{
	public class FamilyCsv
	{
		public string Name { get; set; }
		public string StreetAddress { get; set; }
		public string City { get; set;}
		public string State { get; set; }
		public string Zip { get; set; }
		public int DueDay { get; set; }
		public int NumChildren { get; set; }
		public int BillableDays { get; set; }
		public string Disposition { get; set; } // New = 0, Returning = 1
		public string IsGraduating { get; set; }
		public string CheckSHA256 { get; set; }
		public DateTime? Joined { get; set; }
		public DateTime? Departed { get; set; }
		public string Parent1 { get; set; }
		public string Email1 { get; set; }
		public string Parent2 { get; set; }
		public string Email2 { get; set; }
	}
}

