using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<Fee> LoadAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Fee>("SELECT * FROM Fee"))
					yield return record;
			}
		}

		public Fee Load(long id)
		{
			Fee record = null;

			using (IDbConnection con = _dbFactory.Open())
				record = con.Query<Fee>("SELECT * FROM Fee WHERE Id=?", new { id }).SingleOrDefault();

			return record;
		}

		public long GetIdForCode(string code)
		{
			long id = -1;

			using (IDbConnection con = _dbFactory.Open())
				id = con.ExecuteScalar<long>("SELECT Id FROM Fee WHERE Code=?", new { code });

			return id;
		}

		public void Remove(long id)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("DELETE FROM Fee WHERE Id=?", new { id });
		}

	}
}
