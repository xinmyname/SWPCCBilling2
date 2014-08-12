using System;
using SWPCCBilling2.Infrastructure;
using System.Text;

namespace SWPCCBilling2.Completions
{
	public class NoCompletion : ICompleteText
	{
		private static NoCompletion _default;

		public static NoCompletion Default
		{
			get 
			{
				if (_default == null)
					_default = new NoCompletion();

				return _default;
			}
		}

		private NoCompletion()
		{
		}

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

