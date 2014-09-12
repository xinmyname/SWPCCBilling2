using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Infrastructure
{
	public class MICRStore
	{
		private readonly DatabaseFactory _dbFactory;

		public MICRStore()
		{
			_dbFactory = new DatabaseFactory();
		}

		public void Save(MICR micr, string familyName)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM MICR WHERE CheckSHA256=? AND FamilyName=?", new { micr.SHA256, familyName});

				if (count > 0)
					return;

				con.Execute("INSERT INTO MICR Values (?,?)", new { micr.SHA256, familyName });
			}
		}

		public IEnumerable<string> LoadFamilyNamesForMICR(MICR micr)
		{
			var records = new List<string>();

			using (IDbConnection con = _dbFactory.Open())
			using (IDbCommand cmd = con.CreateCommand("SELECT FamilyName FROM MICR WHERE CheckSHA256=?"))
			{
				cmd.AddParameter(null, micr.SHA256);

				IDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
					records.Add(dr.GetString(0));
			}

			return records;
		}
	}
	
}
