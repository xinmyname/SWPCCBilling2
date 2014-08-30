using System;

namespace SWPCCBilling2.Infrastructure
{

	public class Error : ApplicationException
	{
		public Error(string format, params object[] args)
			: base(String.Format(format, args))
		{
		}

		public Error(Exception innerException, string format, params object[] args)
			: base(String.Format(format, args), innerException)
		{
		}
	}
}
