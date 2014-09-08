using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Infrastructure
{
	public class PaymentStore
	{
		public DatabaseFactory _dbFactory;

		public PaymentStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public void Add(Payment payment)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO Payment VALUES (NULL,?,?,?,?,?,?)", payment.NonKeyValues());
				payment.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM Payment");
			}
		}

		public void Remove(Payment payment)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("DELETE FROM Payment WHERE Id=?", new { payment.Id });
		}

		public IEnumerable<Payment> LoadUndeposited()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Payment>("SELECT * FROM Payment WHERE DepositId IS NULL ORDER BY FamilyName"))
					yield return record;
			}
		}

		public IEnumerable<Payment> LoadPaymentsForInvoice(Invoice invoice)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Payment>("SELECT * FROM Payment WHERE InvoiceId=?", new { invoice.Id }))
					yield return record;
			}
		}

		public void Save(IEnumerable<Payment> payments)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (Payment payment in payments)
				{
					con.Execute("UPDATE Payment SET FamilyName=?,CheckNum=?,Amount=?,Received=?,InvoiceId=?,DepositId=? WHERE Id=?",
						payment.AllValuesKeyLast());
				}
			}
		}
	}
}
