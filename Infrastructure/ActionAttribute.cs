using System;

namespace SWPCCBilling2.Infrastructure
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ActionAttribute : Attribute
	{
		public string Name { get; set; }
		public string ParamTemplate { get; set;}

		public ActionAttribute(string name, string paramTemplate)
		{
			Name = name;
			ParamTemplate = paramTemplate;
		}

		public ActionAttribute(string name)
		{
			Name = name;
			ParamTemplate = null;
		}
	}
}

