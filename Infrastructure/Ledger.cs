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

		public void Debit(string familyName, string feeCode, long quantity, double unitPrice, string notes)
		{
			throw new NotImplementedException();
		}

		public void Credit(string familyName, string feeCode, long quantity, double unitPrice, string notes, long? paymentId = null)
		{
			throw new NotImplementedException();
		}

		public IList<LedgerLine> LoadLinesWithoutInvoiceForFamily(string familyName)
		{
			throw new NotImplementedException();
		}

		public void SaveLine(LedgerLine line)
		{
			throw new NotImplementedException();
		}
	}
}
