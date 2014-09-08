using System;
using System.Data;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Linq;
using EmbeddedResources;
using System.Reflection;

namespace SWPCCBilling2.Infrastructure
{

	public class DepositStore
	{
		private readonly DatabaseFactory _dbFactory;
		private readonly EmbeddedResourceLoader _resLoader;


		public DepositStore()
		{
			_dbFactory = new DatabaseFactory();

			Assembly asm = Assembly.GetExecutingAssembly();
			ILocateResources resLocator = new AssemblyResourceLocator(asm);
			_resLoader = new EmbeddedResourceLoader(resLocator);
		}

		public IEnumerable<Deposit> LoadAll()
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				foreach (var record in con.Query<Deposit>("SELECT * FROM Deposit ORDER BY Date DESC"))
					yield return record;
			}
		}

		public void Add(Deposit deposit)
		{
			using (IDbConnection con = _dbFactory.Open())
			{
				con.Execute("INSERT INTO [Deposit] VALUES (NULL,?,?)", new { deposit.Date, deposit.Amount });
				deposit.Id = con.ExecuteScalar<long>("SELECT MAX(Id) FROM [Deposit]");
			}
		}

		public IDictionary<long,IList<long>> InvoicesForDeposits(DateTime month)
		{
			var invoicesForDeposits = new Dictionary<long, IList<long>>();

			using (IDbConnection con = _dbFactory.Open())
			using (IDbCommand cmd = con.CreateCommand(_resLoader.LoadText("InvoicesForDeposits.sql"), new { month }))
			using (IDataReader reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					long depositId = reader.GetInt64(0);
					string invoiceIdsText = reader.GetString(1);
					IList<long> invoiceIds = invoiceIdsText.Split(',').Select(Int64.Parse).ToList();

					invoicesForDeposits[depositId] = invoiceIds;
				}
			}

			return invoicesForDeposits;
		}

		public Deposit Load(long id)
		{
			Deposit record = null;

			using (IDbConnection con = _dbFactory.Open())
				record = con.Query<Deposit>("SELECT * FROM Deposit WHERE Id=?", new { id }).SingleOrDefault();

			return record;
		}
	}
}
