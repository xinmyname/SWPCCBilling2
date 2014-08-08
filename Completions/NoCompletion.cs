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

		public void TrunkInsert(int position, char ch)
		{
		}

		public void TrunkRemove(int position)
		{
		}

		public string Next()
		{
			return null;
		}

		public string Trunk
		{
			get { return null; }
		}

		public int TrunkLength
		{
			get { return 0; }
		}
	}
}

