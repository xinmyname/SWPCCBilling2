using System;
using System.IO;
using CsvHelper;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Infrastructure
{
	public class FamilyImporter
	{
		public Action<Family> FamilyAction { get; set; }
		public Action<Parent> ParentAction { get; set; }

		public FamilyImporter()
		{
		}

		public void ImportCsvAtPath(string path)
		{
			var reader = new CsvReader(new StreamReader(path));

			foreach (FamilyCsv csvRecord in reader.GetRecords<FamilyCsv>())
			{
				Family family = FamilyFromFamilyCsv(csvRecord);

				FamilyAction(family);

				Parent parent = ParentForFamily(family.Id, csvRecord.Parent1, csvRecord.Email1);

				if (parent != null)
					ParentAction(parent);

				parent = ParentForFamily(family.Id, csvRecord.Parent2, csvRecord.Email2);

				if (parent != null)
					ParentAction(parent);
			}
		}

		public Family FamilyFromFamilyCsv(FamilyCsv csvRecord)
		{
			throw new NotImplementedException();
		}

		public Parent ParentForFamily(long familyId, string name, string email)
		{
			if (familyId == -1 || String.IsNullOrEmpty(name))
				return null;

			throw new NotImplementedException();
		}
	}
}

