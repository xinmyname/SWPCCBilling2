using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{

	public class InvoiceModule : NancyModule
	{
		public InvoiceModule(InvoiceViewModelFactory factory)
		{
			Get["/invoice/{id}/{view?}"] = _ =>
			{
				string viewName = _.view;
				if (viewName == null)
					viewName = "Monthly";
				InvoiceViewModel viewModel = factory.Create(_.id);
				return View[viewName, viewModel];
			};
		}
	}
}
