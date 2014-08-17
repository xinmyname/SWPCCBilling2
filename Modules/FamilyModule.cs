using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{
	public class FamilyModule : NancyModule
	{
		public FamilyModule(FamilyStore familyStore)
		{
			Get["/family/{name}"] = _ => {
				string name = _.name;
				var model = familyStore.Load(name);
				return View["Index", model];
			};
		}
	}
}
