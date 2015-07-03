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
			{
				con.Execute("DELETE FROM Payment WHERE Id=?", new { payment.Id });
			}
		}

		public void Rescind(Payment payment)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("DELETE FROM Payment WHERE Id=?", new { payment.Id });
				con.Execute("DELETE FROM LedgerLine WHERE PaymentId=?", new { payment.Id });
				con.Execute("UPDATE Invoice SET Closed=NULL WHERE Id=?", new { payment.InvoiceId });
				con.Execute("DELETE FROM InvoiceLine WHERE FeeCode='Payment' AND InvoiceId=? AND UnitPrice=?", 
					new { 
						payment.InvoiceId, 
						UnitPrice=-payment.Amount 
					});
			}
		}

		public IEnumerable<Payment> LoadUndeposited()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Payment>("SELECT * FROM Payment WHERE DepositId IS NULL ORDER BY FamilyName"))
					yield return record;
			}
		}

		public IEnumerable<Payment> LoadForDepositId(long depositId)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Payment>("SELECT * FROM Payment WHERE DepositId=? ORDER BY FamilyName", new { depositId }))
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

		public IEnumerable<Payment> LoadUndepositedPaymentsForFamily(Family family)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Payment>("SELECT * FROM Payment WHERE DepositId IS NULL AND FamilyName=?", new { family.Name }))
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
