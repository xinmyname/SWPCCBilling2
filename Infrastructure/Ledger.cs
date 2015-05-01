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

		public static long LastLedgerId = -1;

		public Ledger()
		{
			_dbFactory = new DatabaseFactory();
		}

		public LedgerLine Debit(Family family, Fee fee, long? quantity, double? amount)
		{
			// Id - AddLine()
			// FamilyName - family.Name
			// Date - LedgerLine()
			// InvoiceId - null
			// PaymentId - null
			// FeeCode - fee.Code
			// Quantity - quantity
			// UnitPrice - amount
			// Notes - null

			var ledgerLine = new LedgerLine(family, fee, quantity, amount, false);

			return AddLine(ledgerLine);
		}

		public LedgerLine Debit(Family family, Fee fee, Invoice invoice, decimal amount)
		{
			// Id - AddLine()
			// FamilyName - family.Name
			// Date - LedgerLine()
			// InvoiceId - invoice.Id
			// PaymentId - null
			// FeeCode - fee.Code
			// Quantity - 1
			// UnitPrice - amount
			// Notes - null

			var ledgerLine = new LedgerLine(family, fee, 1, (double)amount, false) {
				InvoiceId = invoice.Id
			};

			return AddLine(ledgerLine);
		}

		public LedgerLine Credit(Family family, Fee fee, long? quantity, double? amount)
		{
			// Id - AddLine()
			// FamilyName - family.Name
			// Date - LedgerLine()
			// InvoiceId - null
			// PaymentId - null
			// FeeCode - fee.Code
			// Quantity - quantity
			// UnitPrice - amount
			// Notes - null

			var ledgerLine = new LedgerLine(family, fee, quantity, amount, true);

			return AddLine(ledgerLine);
		}

		public LedgerLine Credit(Family family, Fee fee, Invoice invoice, Payment payment)
		{
			// Id - AddLine()
			// FamilyName - family.Name
			// Date - LedgerLine()
			// InvoiceId - invoice.Id
			// PaymentId - payment.Id
			// FeeCode - fee.Code
			// Quantity - 1
			// UnitPrice - payment.Amount
			// Notes - null

			var ledgerLine = new LedgerLine(family, fee, 1, payment.Amount, true) {
				InvoiceId = invoice.Id,
				PaymentId = payment.Id
			};

			return AddLine(ledgerLine);
		}

		public LedgerLine Credit(Family family, Fee fee, decimal amount)
		{
			// Id - AddLine()
			// FamilyName - family.Name
			// Date - LedgerLine()
			// InvoiceId - null
			// PaymentId - null
			// FeeCode - fee.Code
			// Quantity - 1
			// UnitPrice - amount
			// Notes - null

			var ledgerLine = new LedgerLine(family, fee, 1, (double)amount, true);

			return AddLine(ledgerLine);
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

		public void AddNoteToLastEntry(string note)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("UPDATE LedgerLine SET Notes=? WHERE Id=?", new { note, LastLedgerId });
		}

		private LedgerLine AddLine(LedgerLine line)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [LedgerLine] VALUES (NULL,?,?,?,?,?,?,?,?)", line.NonKeyValues());
				line.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [LedgerLine]");
				LastLedgerId = line.Id;
			}

			return line;
		}
	}
}
