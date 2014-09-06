using System;
using Nancy;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Modules
{
	public class ReportModule : NancyModule
	{
		public ReportModule(PaymentStore paymentStore, InvoiceStore invoiceStore, ParentStore parentStore)
		{
			Get["/report/deposit/pending"] = _ =>
			{
				IList<Payment> undeposited = paymentStore.LoadUndeposited().ToList();
				decimal depositAmount = undeposited.Sum(x => (decimal)x.Amount);

				return View["DepositPending", new {
					Checks = undeposited.Select(p => new {
						p.FamilyName,
						p.CheckNum,
						p.Amount,
						AmountText = p.Amount.ToString("C")
					}),
					Amount = depositAmount,
					AmountText = depositAmount.ToString("C")
				}];
			};

			Get["/report/deposit/{id}"] = _ =>
			{
				throw new NotImplementedException();
			};

			Get["/report/unpaid/{date}"] = _ =>
			{
				IList<Invoice> unpaidInvoices = invoiceStore.LoadOpenInvoicesAfter(_.date);

				IEnumerable<string> unpaidEmails = unpaidInvoices
					.SelectMany(i => 
						parentStore
							.LoadForFamilyName(i.FamilyName)
							.Select(p => p.Email)
							.Where(e => e != null));

				return View["Unpaid", unpaidEmails];
			};

		}
	}
}
