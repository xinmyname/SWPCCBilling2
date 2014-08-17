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
			object[] values = record.AllValues();

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Fee] VALUES (?,?,?,?,?)", values);
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

		public Fee Load(string code)
		{
			Fee record = null;

			using (IDbConnection con = _dbFactory.Open())
				record = con.Query<Fee>("SELECT * FROM Fee WHERE Code=?", new { code }).SingleOrDefault();

			return record;
		}

		public void Remove(string code)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("DELETE FROM Fee WHERE Code=?", new { code });
		}

	}
}
