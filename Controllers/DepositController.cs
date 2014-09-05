using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using SWPCCBilling2.Models;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SWPCCBilling2.Controllers
{
	public class DepositController : Controller
	{
		private readonly UrlFactory _urlFactory;

		public DepositController()
		{
			_urlFactory = UrlFactory.DefaultUrlFactory;
		}

		[Action("show-pending")]
		public void ShowPendingDeposit()
		{
			string url = _urlFactory.UrlForPath("deposit");

			Process.Start(url);
		}

		[Action("deposit-payment")]
		public void DepositPayment()
		{
			throw new NotImplementedException();
		}
	}
	
}
