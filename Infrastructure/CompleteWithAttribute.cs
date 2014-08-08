using System;

namespace SWPCCBilling2.Infrastructure
{

	[AttributeUsage(AttributeTargets.Parameter)]
	public class CompleteWithAttribute : Attribute
	{
		public Type Type { get; set; }

		public CompleteWithAttribute(Type type)
		{
			Type = type;
		}
	}
}
