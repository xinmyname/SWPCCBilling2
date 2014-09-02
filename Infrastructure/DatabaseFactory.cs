using System;
using System.Data;
using System.Reflection;
using System.IO;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using EmbeddedResources;
using Mono.Data.Sqlite;

namespace SWPCCBilling2.Infrastructure
{
	public class DatabaseFactory
	{
		readonly string _dbFileName;

        public DatabaseFactory()
		{
			var settingsStore = SettingsStore.DefaultSettingsStore;
            Settings settings = settingsStore.Load();
            _dbFileName = settings.DatabaseName;
		}

        public IDbConnection Open()
        {
            IDbConnection con = null;
            string dbPath = DocumentPath.For(_dbFileName);
            bool dbExists = File.Exists(dbPath);

			if (!dbExists)
				Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

			con = new SqliteConnection();
			con.ConnectionString = String.Format("URI=file:{0},version=3", dbPath);

            con.Open();

			if (!dbExists)
				CreateEmptyDatabase(con);

			return con;
        }

		private void CreateEmptyDatabase(IDbConnection con)
		{
			Console.WriteLine("Creating new database...");

			using (IDbTransaction xact = con.BeginTransaction())
			{
				var asm = Assembly.GetExecutingAssembly();
				var resLocator = new AssemblyResourceLocator(asm);
				var loader = new EmbeddedResourceLoader(resLocator);
				string sql = loader.LoadText("CreateSchema.sql");

				IEnumerable<string> statements = Regex.Split(sql.Replace("\r\n", "\n"), ";\n")
					.Select(s => s.Trim())
					.Where(s => !String.IsNullOrEmpty(s));

				foreach (string statement in statements)
				{
					using (IDbCommand cmd = con.CreateCommand())
					{
						cmd.Transaction = xact;
						cmd.CommandText = statement;
						cmd.ExecuteNonQuery();
					}
				}

				xact.Commit();
			}
		}
	}
}

