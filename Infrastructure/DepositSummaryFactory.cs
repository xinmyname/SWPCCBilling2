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
		private readonly IDictionary<Deposit, IDictionary<string, double>> _deposits;
		private readonly HashSet<string> _headers;

		public DepositSummaryFactory()
		{
			_deposits = new Dictionary<Deposit, IDictionary<string, double>>();
			_headers = new HashSet<string>();
		}

		public void AddDeposit(Deposit deposit, IDictionary<string, double> categoryTotals)
		{
			_deposits[deposit] = categoryTotals;

			foreach (string category in categoryTotals.Keys)
				_headers.Add(category);
		}

		public IEnumerable<string> BuildHeaderRow()
		{
			yield return @"<tr>";
			yield return @"<th>Deposit</th>";
			yield return @"<th>Date</th>";

			foreach (string header in _headers.OrderByDescending(h => h))
				yield return String.Format(@"<th class=""money"">{0}</th>", header);
	
			yield return @"<th class=""money"">Total</th>";
			yield return @"</tr>";
		}

		public IEnumerable<string> BuildDepositRows()
		{
			IList<string> headers = _headers.OrderByDescending(h => h).ToList();
			IList<Deposit> deposits = _deposits.Keys.OrderBy(d => d.Date).ToList();

			foreach (Deposit deposit in deposits)
			{
				IDictionary<string, double> categoryTotals = _deposits[deposit];

				yield return @"<tr>";
				yield return String.Format(@"<td>{0}</td>", deposit.Id);
				yield return String.Format(@"<td>{0:d}</td>", deposit.Date);

				decimal total = 0;

				foreach (string header in headers)
				{
					decimal value = 0;

					if (categoryTotals.ContainsKey(header))
					{
						value = (decimal)categoryTotals[header];
						total += value;
					}

					yield return String.Format(@"<td class=""money"">{0}</td>", value.ToHtmlCurrency());
				}

				yield return String.Format(@"<td class=""money"">{0}</td>", total.ToHtmlCurrency());

				yield return @"</tr>";
			}
		}

		public void Clear()
		{
			_deposits.Clear();
			_headers.Clear();
		}
	}
}
