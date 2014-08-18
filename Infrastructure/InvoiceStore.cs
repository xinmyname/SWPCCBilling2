using System;
using SWPCCBilling2.Models;
using System.Data;
using System.Linq;

namespace SWPCCBilling2.Infrastructure
{
	public class InvoiceStore
	{
		private readonly DatabaseFactory _dbFactory;

		public InvoiceStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public Invoice Load(string familyName, DateTime date)
		{
			Invoice record = null;

			using (IDbConnection con = _dbFactory.Open())
			{
				record = con.Query<Invoice>("SELECT * FROM Invoice WHERE FamilyName=? AND Opened=?", new { familyName, date }).SingleOrDefault();

				if (record != null)
					record.Lines = con.Query<InvoiceLine>("SELECT * FROM InvoiceLine WHERE InvoiceId=?", new { record.Id }).ToList();
			}

			return record;
		}

		public void Add(Invoice invoice)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				object values = new { invoice.FamilyName, invoice.Due, invoice.Sent, invoice.Opened, invoice.Closed };
				con.Execute("INSERT INTO [Invoice] VALUES (NULL,?,?,?,?,?)", values);
				invoice.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [Invoice]");

				foreach (InvoiceLine line in invoice.Lines)
				{
					values = line.AllValues();
					con.Execute("INSERT INTO [InvoiceLine] VALUES(?,?,?,?,?,?)", values);
				}
			}
		}

		public void Save(Invoice invoice)
		{
			throw new NotImplementedException();
		}
	}
}

