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
			object[] values = record.NonKeyValues();

			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Family] VALUES (NULL,?,?,?,?,?,?,?,?,?,?,?,?,?)", values);
				record.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [Family]");
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

		public Family Load(long id)
		{
			Family record = null;

			using (IDbConnection con = _dbFactory.Open())
				record = con.Query<Family>("SELECT * FROM Family WHERE Id=?", new { id }).SingleOrDefault();

			return record;
		}

		public long GetIdForName(string name)
		{
			long id = -1;

			using (IDbConnection con = _dbFactory.Open())
				id = con.ExecuteScalar<long>("SELECT Id FROM Family WHERE Name=?", new { name });

			return id;
		}

		public void Remove(long id)
		{
			using (IDbConnection con = _dbFactory.Open())
				con.Execute("UPDATE Family SET Departed=date(\"now\") WHERE Id=?", new { id });
		}

		public bool Save(Family record)
		{
			int rows = 0;

			using (IDbConnection con = _dbFactory.Open())
			{
				rows = con.Execute("UPDATE Family SET Name=?,StreetAddress=?,City=?,State=?,Zip=?,DueDay=?,NumChildren=?,BillableDays=?,IsNew=?,IsGraduating=?,CheckSHA256=?,Joined=?,Departed=?",
					record.AllValuesKeyLast());
			}

			return rows != 0;
		}
	}
}

