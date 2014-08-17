using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{

	public class FeeModule : NancyModule
	{
		public FeeModule(FeeStore feeStore)
		{
			Get["/fee/{code}"] = _ => {
				string code = _.code;
				var model = feeStore.Load(code);
				return View["Index", model];
			};
		}
	}
}
