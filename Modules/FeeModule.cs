using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{

	public class FeeModule : NancyModule
	{
		public FeeModule(FeeStore feeStore)
		{
			Get["/fee/{id}"] = _ => {
				long id = _.id;
				var model = feeStore.Load(id);
				return View["Index", model];
			};
		}
	}
}
