using System;
using SWPCCBilling2.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Nancy;

namespace SWPCCBilling2.Infrastructure
{

	public class DepositSummaryFactory
	{
		public void AddDeposit(Deposit deposit, IDictionary<string, double> categoryTotals)
		{
		}

		public IEnumerable<string> BuildHeaderRow()
		{
			yield return @"<tr>";
			yield return @"<th>Deposit</th>";
			yield return @"<th>Date</th>";
			yield return @"<th class=""money"">Tuition</th>";
			yield return @"<th class=""money"">Insurance</th>";
			yield return @"<th class=""money"">Credits</th>";
			yield return @"<th class=""money"">Total</th>";
			yield return @"</tr>";
		}

		public IEnumerable<string> BuildDepositRows()
		{
			yield return @"<tr>";
			yield return @"<td>1</td>";
			yield return @"<td>1/1/2010</td>";
			yield return @"<td class=""money"">$4000.00</td>";
			yield return @"<td class=""money"">$500.00</td>";
			yield return @"<td class=""money"">$0.00</td>";
			yield return @"<td class=""money"">$4500.00</td>";
			yield return @"</tr>";
			yield return @"<tr>";
			yield return @"<td>2</td>";
			yield return @"<td>1/2/2010</td>";
			yield return @"<td class=""money"">$3000.00</td>";
			yield return @"<td class=""money"">$200.00</td>";
			yield return @"<td class=""money"">$0.00</td>";
			yield return @"<td class=""money"">$3200.00</td>";
			yield return @"</tr>";
		}

		public void Clear()
		{
		}
	}
}
