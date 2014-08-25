using System;
using Nancy;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2.Modules
{

	public class InvoiceModule : NancyModule
	{
		public InvoiceModule(InvoiceViewModelFactory factory)
		{
			Get["/invoice/{id}"] = _ =>
			{
				InvoiceViewModel viewModel = factory.Create(_.id);
				return View["Monthly", viewModel];
			};
		}
	}
}
