using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

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
			object[] values = record.AllValues();

			using (IDbConnection con = _dbFactory.Open())
				con.Execute("INSERT INTO [Parent] VALUES (?,?,?)", values);
		}

		public IList<Parent> LoadForFamilyName(string familyName)
		{
			var parents = new List<Parent>();

			using (IDbConnection con = _dbFactory.Open())
				parents = con.Query<Parent>("SELECT * FROM [Parent] WHERE FamilyName=?", new { familyName }).ToList();

			return parents;
		}
	}
}
