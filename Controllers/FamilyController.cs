using System;
using SWPCCBilling2.Infrastructure;
using SWPCCBilling2.Completions;
using System.Diagnostics;
using SWPCCBilling2.Models;
using System.Runtime.InteropServices;

namespace SWPCCBilling2.Controllers
{
	public class FamilyController : Controller
	{
		private readonly SettingsStore _settingsStore;
		private readonly FamilyStore _familyStore;
		private readonly UrlFactory _urlFactory;

		public FamilyController()
		{
			_settingsStore = new SettingsStore();
			_familyStore = new FamilyStore();
			_urlFactory = UrlFactory.DefaultUrlFactory;
		}
		
		[Action("import-families", "path-to-csv-file")]
		public void ImportFamilies(
			[Optional]string path)
		{
			if (path == null)
			{
				Settings settings = _settingsStore.Load();
				path = settings.DefaultFamilyImportPath;
			}

			var parentStore = new ParentStore();
			var importer = new FamilyImporter();

			_familyStore.RemoveAll();
			parentStore.RemoveAll();

			importer.FamilyAction = _familyStore.Add;
			importer.ParentAction = parentStore.Add;

			importer.ImportCsvAtPath(path);
		}

		[Action("show-family", "family-name")]
		public void ShowFamily(
			[CompleteWith(typeof(FamilyCompletion))] string familyName)
		{
			string url = _urlFactory.UrlForPath("family/{0}", familyName);

			Process.Start(url);
		}

		[Action("remove-family", "family-name")]
		public void RemoveFamily(
			[CompleteWith(typeof(FamilyCompletion))] string familyName)
		{
			_familyStore.Remove(familyName);
		}

		[Action("import-micr", "family-name")]
		public void ImportMICR(
			[CompleteWith(typeof(FamilyCompletion))] string familyName)
		{
			Console.WriteLine("Insert check into scanner...");
			string micrText = Console.ReadLine();
			var micr = new MICR(micrText);

			if (!micr.IsValid)
				throw new ApplicationException("Could not read MICR data from check.");

			Console.WriteLine(micr);

			Family family = _familyStore.Load(familyName);

			family.CheckSHA256 = micr.SHA256;

			_familyStore.Save(family);
		}
	}
}

