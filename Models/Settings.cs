using System;

namespace SWPCCBilling2.Models
{
	public class Settings
	{
		public string DatabaseName { get; set; }
		public string EmailServer { get; set; }
		public int EmailPort { get; set; }
		public bool EmailSecure { get; set; }
		public string EmailFrom { get; set; }
	}
}

