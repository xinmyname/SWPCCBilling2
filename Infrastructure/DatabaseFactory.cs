using System;
using System.Data;
using System.Reflection;
using System.IO;
using SWPCCBilling2.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using EmbeddedResources;

namespace SWPCCBilling2.Infrastructure
{
	public class DatabaseFactory
	{
		readonly string _dbFileName;

        public DatabaseFactory(SettingsStore settingsStore)
		{
            Settings settings = settingsStore.Load();
            _dbFileName = settings.DatabaseName;
		}

        public IDbConnection Open()
        {
            IDbConnection con = null;
            string dbPath = DocumentPath.For(_dbFileName);
            bool dbExists = File.Exists(dbPath);
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            if (Type.GetType("Mono.Runtime") != null)
            {
                Assembly asm = Assembly.Load("Mono.Data.Sqlite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
                Type type = asm.GetType("Mono.Data.Sqlite.SqliteConnection");
                con = (IDbConnection) Activator.CreateInstance(type);
                con.ConnectionString = String.Format("URI=file:{0},version=3", dbPath);
            }
            else
            {
                Assembly asm = Assembly.Load("System.Data.SQLite, Version=1.0.93.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139");
                Type type = asm.GetType("System.Data.SQLite.SQLiteConnection");
                con = (IDbConnection) Activator.CreateInstance(type);
                con.ConnectionString = String.Format("Data Source={0};Version=3", dbPath);
            }

            con.Open();

            if (!dbExists)
            {
                Console.WriteLine("Creating new database...");

                IDbTransaction xact = con.BeginTransaction();

                var asm = Assembly.GetExecutingAssembly();
                var resLocator = new AssemblyResourceLocator(asm);
                var loader = new EmbeddedResourceLoader(resLocator);
                string sql = loader.LoadText("CreateSchema.sql");

                IEnumerable<string> statements = Regex.Split(sql.Replace("\r\n", "\n"), ";\n")
                    .Select(s => s.Trim())
                    .Where(s => !String.IsNullOrEmpty(s));

                foreach (string statement in statements)
                {
                    IDbCommand cmd = con.CreateCommand();
                    cmd.Transaction = xact;
                    cmd.CommandText = statement;
                    cmd.ExecuteNonQuery();
                }

                xact.Commit();
            }

            return con;
        }
	}
}

