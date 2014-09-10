using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Controllers
{
	public class NotificationController : Controller
	{
		[Action("send-2day-warning","family-name(optional)")]
		public void SendTwoDayWarning(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("send-2day-violation","family-name(optional)")]
		public void SendTwoDayViolation(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("send-7day-warning","family-name(optional)")]
		public void SendSevenDayWarning(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("send-7day-violation","family-name(optional)")]
		public void SendSevenDayViolation(
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			throw new NotImplementedException();
		}
	}
	
}
