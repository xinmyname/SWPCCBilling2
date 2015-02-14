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
		private readonly DateFactory _dateFactory;

		public FamilyStore()
		{
			_dbFactory = new DatabaseFactory();
			_dateFactory = DateFactory.DefaultDateFactory;
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
				foreach (var record in con.Query<Family>("SELECT * FROM Family ORDER BY Name"))
					yield return record;
			}
		}

		public IEnumerable<Family> LoadActive()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Family>("SELECT * FROM Family WHERE Departed IS NULL OR NOT Departed < " +
					"date(\"now\") ORDER BY Name"))
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
				con.Execute("UPDATE Family SET Departed=? WHERE Name=?", new { departed=_dateFactory.Now(), name });
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

		public IEnumerable<Family> LoadActiveFamilesWithName(string name)
		{
			switch (name.ToUpper())
			{
				case "ALL":
					return LoadActive();
				case "RETURNING":
					return LoadActive().Where(f => !f.IsNew);
				case "NEW":
					return LoadActive().Where(f => f.IsNew);
				case "GRADUATING":
					return LoadActive().Where(f => f.IsGraduating);
				default:
					return new[]{ Load(name) };
			}
		}

		public void RenameFamily(string oldName, string newName)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("UPDATE Family SET Name=? WHERE Name=?", new { newName, oldName });
				con.Execute("UPDATE Parent SET FamilyName=? WHERE FamilyName=?", new { newName, oldName });
				con.Execute("UPDATE Discount SET FamilyName=? WHERE FamilyName=?", new { newName, oldName });
				con.Execute("UPDATE MICR SET FamilyName=? WHERE FamilyName=?", new { newName, oldName });
				con.Execute("UPDATE Invoice SET FamilyName=? WHERE FamilyName=?", new { newName, oldName });
				con.Execute("UPDATE LedgerLine SET FamilyName=? WHERE FamilyName=?", new { newName, oldName });
				con.Execute("UPDATE Payment SET FamilyName=? WHERE FamilyName=?", new { newName, oldName });
			}
		}
	}
}

