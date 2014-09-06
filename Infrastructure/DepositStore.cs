using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Infrastructure
{

	public class DepositStore
	{
		public DatabaseFactory _dbFactory;

		public DepositStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public IEnumerable<Deposit> LoadAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Deposit>("SELECT * FROM Deposit ORDER BY Date DESC"))
					yield return record;
			}
		}
	}
}
