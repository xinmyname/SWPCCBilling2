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
		private readonly MICRStore _micrStore;

		public FamilyController()
		{
			_settingsStore = SettingsStore.DefaultSettingsStore;
			_familyStore = new FamilyStore();
			_urlFactory = UrlFactory.DefaultUrlFactory;
			_micrStore = new MICRStore();
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
			[CompleteWith(typeof(FamilyCompletion))][Optional] string familyName)
		{
			string url = familyName != null
				? _urlFactory.UrlForPath("family/{0}", familyName)
				: _urlFactory.UrlForPath("families");

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
			Family family = _familyStore.Load(familyName);

			if (family == null)
				throw new Error("{0} family not found in database.");

			Console.WriteLine("Insert check into scanner...");
			string micrText = Console.ReadLine();
			var micr = new MICR(micrText);

			if (!micr.IsValid)
				throw new Error("Could not read MICR data from check.");

			Console.WriteLine(micr);

			_micrStore.Save(micr, family.Name);
		}
	}
}

