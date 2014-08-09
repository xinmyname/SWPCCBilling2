using System;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2
{
	public class NoCompletion : ICompleteText
	{
		public void Preload()
		{
		}
			
		public string Next()
		{
			return null;
		}

		public string Trunk
		{
			get { return null; }
			set { }
		}
	}
}

