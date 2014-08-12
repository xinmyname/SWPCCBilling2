using System;
using System.Data;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Infrastructure
{
	public class ParentStore
	{
		private readonly DatabaseFactory _dbFactory;

		public ParentStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public void RemoveAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("DELETE FROM Parent");
			}
		}

		public void Add(Parent record)
		{
			object[] values = record.NonKeyValues();

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Parent] VALUES (NULL,?,?,?)", values);
				record.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [Parent]");
			}
		}
	}
}
