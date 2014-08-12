using System;
using System.IO;
using CsvHelper;
using SWPCCBilling2.Models;

namespace SWPCCBilling2.Infrastructure
{
	public class FeeImporter
	{
		public Action<Fee> FeeAction { get; set; }

		public void ImportCsvAtPath(string path)
		{
			var reader = new CsvReader(new StreamReader(path));

			foreach (var feeCsv in reader.GetRecords<FeeCsv>())
			{
				var fee = new Fee 
				{
					Code = feeCsv.Code,
					Description = feeCsv.Description,
					Type = feeCsv.Type,
					Category = feeCsv.Category,
					Amount = feeCsv.Amount
				};

				FeeAction(fee);
			}
		}
	}
	
}
