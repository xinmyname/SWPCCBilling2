using System;
using SWPCCBilling2.Infrastructure;
using System.Runtime.InteropServices;

namespace SWPCCBilling2
{
	public class InvoiceController : Controller
	{
		public InvoiceController()
		{
		}

		[Action("open-invoice","family-name date(optional)")]
		public void OpenInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			throw new NotImplementedException();
		}

		[Action("close-invoice","family-name date(optional)")]
		public void CloseInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			throw new NotImplementedException();
		}

		[Action("send-invoice","family-name date(optional)")]
		public void SendInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			throw new NotImplementedException();
		}

		[Action("rebuild-invoice","family-name date(optional)")]
		public void RebuildInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))][Optional] DateTime? date)
		{
			throw new NotImplementedException();
		}

		[Action("remove-invoice","family-name date")]
		public void RemoveInvoice(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			[CompleteWith(typeof(DateCompletion))] DateTime date)
		{
			throw new NotImplementedException();
		}
	}
}

