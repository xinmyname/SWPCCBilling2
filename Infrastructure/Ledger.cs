using System;
using SWPCCBilling2.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace SWPCCBilling2.Infrastructure
{
	public class Ledger
	{
		private readonly DatabaseFactory _dbFactory;

		public Ledger()
		{
			_dbFactory = new DatabaseFactory();
		}

		public LedgerLine Debit(Family family, Invoice invoice, Fee fee, long? qty, double? amt, string notes)
		{
			return Add(family, invoice, fee, qty, amt, notes, false);
		}

		public LedgerLine Credit(Family family, Invoice invoice, Fee fee, long? qty, double? amt, string notes)
		{
			return Add(family, invoice, fee, qty, amt, notes, true);
		}

		public IList<LedgerLine> LoadLinesWithoutInvoiceForFamily(string familyName)
		{
			List<LedgerLine> lines = null;

			using (IDbConnection con = _dbFactory.Open())
				lines = con.Query<LedgerLine>("SELECT * FROM LedgerLine WHERE FamilyName=? AND InvoiceId IS NULL AND PaymentId IS NULL", new { familyName }).ToList();

			return lines;
		}

		public void SaveLine(LedgerLine line)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("UPDATE LedgerLine SET FamilyName=?,Date=?,InvoiceId=?,PaymentId=?,FeeCode=?,Quantity=?,UnitPrice=?,Notes=? WHERE Id=?",
					line.AllValuesKeyLast());
			}
		}

		public void RemoveLine(LedgerLine line)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("DELETE FROM LedgerLine WHERE Id=?", new { line.Id });
		}

		public void RemoveInvoiceId(long invoiceId)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("UPDATE LedgerLine SET InvoiceId=NULL WHERE InvoiceId=?", new { invoiceId });
		}

		private LedgerLine Add(Family family, Invoice invoice, Fee fee, long? qty, double? amt, string notes, bool isCredit)
		{
			double unitPrice = 0.0;
			long quantity = 0;
			long? invoiceId = null;

			if (invoice != null)
				invoiceId = invoice.Id;

			switch (fee.Type)
			{
				case Fee.FeeTypeFixed:
				case Fee.FeeTypePerMinute:
					unitPrice = fee.Amount;
					quantity = qty ?? 1;
					break;
				case Fee.FeeTypeVarying:
					unitPrice = amt ?? 0.0;
					quantity = qty ?? 1;
					break;
				case Fee.FeeTypePerChild:
					unitPrice = fee.Amount;
					quantity = family.NumChildren;
					break;
				case Fee.FeeTypePerChildDay:
					unitPrice = fee.Amount;
					quantity = family.BillableDays;
					break;
			}

			if (isCredit)
				unitPrice = -unitPrice;

			var line = new LedgerLine {
				FamilyName = family.Name,
				Date = DateTime.Now,
				InvoiceId = invoiceId,
				PaymentId = null,
				FeeCode = fee.Code,
				Quantity = quantity,
				UnitPrice = unitPrice,
				Notes = notes
			};

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [LedgerLine] VALUES (NULL,?,?,?,?,?,?,?,?)", line.NonKeyValues());
				line.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [LedgerLine]");
			}

			return line;
		}
	}
}
