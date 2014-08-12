using System;
using System.Data;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Infrastructure
{
	public class FamilyStore
	{
		private readonly DatabaseFactory _dbFactory;

		public FamilyStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public void RemoveAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("DELETE FROM Family");
			}
		}

		public void Add(Family record)
		{
			object[] values = record.NonKeyValues();

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Family] VALUES (NULL,?,?,?,?,?,?,?,?,?,?,?,?,?)", values);
				record.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [Family]");
			}
		}
	}
}

