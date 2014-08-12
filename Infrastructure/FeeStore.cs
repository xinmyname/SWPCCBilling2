using System;
using System.Data;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Infrastructure
{

	public class FeeStore
	{
		private readonly DatabaseFactory _dbFactory;

		public FeeStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public void RemoveAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("DELETE FROM Fee");
			}
		}

		public void Add(Fee record)
		{
			object[] values = record.NonKeyValues();

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Fee] VALUES (NULL,?,?,?,?,?)", values);
				record.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [Fee]");
			}
		}
	}
}
