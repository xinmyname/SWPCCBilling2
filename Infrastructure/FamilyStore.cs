using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

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
			object[] values = record.AllValues();

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Family] VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)", values);
			}
		}

		public IEnumerable<Family> LoadAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Family>("SELECT * FROM Family"))
					yield return record;
			}
		}

		public IEnumerable<Family> LoadActive()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Family>("SELECT * FROM Family WHERE Departed IS NULL OR Departed < date(\"now\")"))
					yield return record;
			}
		}

		public Family Load(string name)
		{
			Family record = null;

			using (IDbConnection con = _dbFactory.Open())
				record = con.Query<Family>("SELECT * FROM Family WHERE Name=?", new { name }).SingleOrDefault();

			return record;
		}

		public void Remove(string name)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("UPDATE Family SET Departed=date(\"now\") WHERE Name=?", new { name });
		}

		public bool Save(Family record)
		{
			int rows = 0;

			using (IDbConnection con = _dbFactory.Open())
			{
				rows = con.Execute("UPDATE Family SET StreetAddress=?,City=?,State=?,Zip=?,DueDay=?,NumChildren=?,BillableDays=?,IsNew=?,IsGraduating=?,CheckSHA256=?,Joined=?,Departed=? WHERE Name=?",
					record.AllValuesKeyLast());
			}

			return rows != 0;
		}

		public IEnumerable<string> GetFamilyNames(string name)
		{
			switch (name.ToUpper())
			{
				case "ALL":
					return LoadActive().Select(f => f.Name);
				case "RETURNING":
					return LoadActive()
						.Where(f => !f.IsNew)
						.Select(f => f.Name);
				case "NEW":
					return LoadActive()
						.Where(f => f.IsNew)
						.Select(f => f.Name);
				case "GRADUATING":
					return LoadActive()
						.Where(f => f.IsGraduating)
						.Select(f => f.Name);
				default:
					return new[]{ name };
			}
		}

	}
}

