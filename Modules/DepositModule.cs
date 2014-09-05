using System;
using Nancy;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Modules
{
	public class DepositModule : NancyModule
	{
		public DepositModule(PaymentStore paymentStore)
		{
			Get["/deposit"] = _ =>
			{
				IList<Payment> undeposited = paymentStore.LoadUndeposited().ToList();
				decimal depositAmount = undeposited.Sum(x => (decimal)x.Amount);

				return View["Index", new {
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
		}
	}
}
