using System;
using SWPCCBilling2.Infrastructure;

namespace SWPCCBilling2
{
	public class FamilyController : Controller
	{
		public FamilyController()
		{
		}
		
		[Action("import-families", "path-to-csv-file")]
		public void ImportFamilies(
			string path)
		{
			throw new NotImplementedException();
		}

		[Action("show-family", "family-name")]
		public void ShowFamily(
			[CompleteWith(typeof(FamilyCompletion))] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("remove-family", "family-name")]
		public void RemoveFamily(
			[CompleteWith(typeof(FamilyCompletion))] string familyName)
		{
			throw new NotImplementedException();
		}

		[Action("import-micr", "family-name MICR")]
		public void ImportMICR(
			[CompleteWith(typeof(FamilyCompletion))] string familyName, 
			string micr)
		{
			throw new NotImplementedException();
		}
	}
}

