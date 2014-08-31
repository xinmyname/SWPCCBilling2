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

		public Invoice Load(long id)
		{
			Invoice record = null;

			using (IDbConnection con = _dbFactory.Open())
			{
				record = con.Query<Invoice>("SELECT * FROM Invoice WHERE Id=?", new { id }).SingleOrDefault();
				LoadLines(con, record);
			}

			return record;
		}

		public Invoice Load(string familyName, DateTime date)
		{
			Invoice record = null;

			using (IDbConnection con = _dbFactory.Open())
			{
				record = con.Query<Invoice>("SELECT * FROM Invoice WHERE FamilyName=? AND Opened=?", new { familyName, date }).SingleOrDefault();
				LoadLines(con, record);
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
					con.Execute("INSERT INTO [InvoiceLine] VALUES(?,?,?,?,?)", values);
				}
			}
		}

		public Invoice LoadLatestOpenInvoiceForFamily(string familyName)
		{
			Invoice record = null;

			using (IDbConnection con = _dbFactory.Open())
			{
				record = con.Query<Invoice>("SELECT * FROM Invoice WHERE FamilyName=? AND Opened IS NOT NULL AND Closed IS NULL ORDER BY Opened DESC", new { familyName }).FirstOrDefault();
				LoadLines(con, record);
			}

			return record;
		}

		public void Save(Invoice invoice, bool withLines = true)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				object values = invoice.AllValuesKeyLast();

				con.Execute("UPDATE Invoice SET FamilyName=?,Due=?,Sent=?,Opened=?,Closed=? WHERE Id=?", values);

				if (withLines)
				{

					con.Execute("DELETE FROM InvoiceLine WHERE InvoiceId=?", new { invoice.Id });

					foreach (InvoiceLine line in invoice.Lines)
						con.Execute("INSERT INTO [InvoiceLine] VALUES(?,?,?,?,?)", line.AllValues());
				}
			}
		}

		public void Remove(Invoice invoice)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("DELETE FROM Invoice WHERE Id=?", new { invoice.Id });
				con.Execute("DELETE FROM InvoiceLine WHERE InvoiceId=?", new { invoice.Id });
			}
		}

		private void LoadLines(IDbConnection con, Invoice invoice)
		{
			if (invoice != null)
				invoice.Lines = con.Query<InvoiceLine>("SELECT * FROM InvoiceLine WHERE InvoiceId=?", new { invoice.Id }).ToList();
		}
	}
}

