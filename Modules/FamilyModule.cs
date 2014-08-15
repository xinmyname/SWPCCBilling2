using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{
	public class FamilyModule : NancyModule
	{
		public FamilyModule(FamilyStore familyStore)
		{
			Get["/family/{id}"] = _ => {
				long id = _.id;
				var model = familyStore.Load(id);
				return View["Show", model];
			};
		}
	}
}
