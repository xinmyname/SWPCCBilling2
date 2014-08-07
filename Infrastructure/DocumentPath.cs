using System;
using System.IO;
using System.Reflection;

namespace SWPCCBilling2.Infrastructure
{
	public static class DocumentPath
	{
		public static string UserDocuments()
		{
			object dataDir = AppDomain.CurrentDomain.GetData("DataDirectory");

			if (dataDir != null)
				return dataDir.ToString();

			string docDir;

			if (Environment.OSVersion.Platform == PlatformID.Unix)
				docDir = String.Format("{0}{1}Documents",
					Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					Path.DirectorySeparatorChar);
			else
				docDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			docDir = docDir + Path.DirectorySeparatorChar + Assembly.GetEntryAssembly().GetName().Name;

			return docDir;
		}

		public static string For(string filename)
		{
			return String.Format("{1}{0}{2}",
				Path.DirectorySeparatorChar,
				UserDocuments(),
				filename);
		}

		public static string For(string subFolder, string filename)
		{
			return String.Format("{1}{0}{2}{0}{3}",
				Path.DirectorySeparatorChar,
				UserDocuments(),
				subFolder,
				filename);
		}

		public static string For(string subFolder1, string subFolder2, string filename)
		{
			return String.Format("{1}{0}{2}{0}{3}{0}{4}",
				Path.DirectorySeparatorChar,
				UserDocuments(),
				subFolder1,
				subFolder2,
				filename);
		}
	}
}

