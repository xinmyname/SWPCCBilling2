using System;
using System.Runtime.Serialization.Json;
using SWPCCBilling2.Models;
using System.IO;

namespace SWPCCBilling2.Infrastructure
{
	public class SettingsStore
	{
		private readonly string _settingsPath;
		private readonly DataContractJsonSerializer _serializer;
		private Settings _settings;

        public static Func<Settings> DefaultSettings;

        static SettingsStore()
        {
            DefaultSettings = () => new Settings() 
            {
                DatabaseName = "SWPCCBilling.sqlite",
				DefaultFamilyImportPath = "/Users/andy/Dropbox/Documents/SWPCC/FY2015/RosterImport.csv",
				DefaultFeeImportPath = "/Users/andy/Dropbox/Documents/SWPCC/FY2015/FeeImport.csv",
				EmailServer = "smtp.gmail.com",
				EmailPort = 587,
				EmailSecure = true,
				EmailFrom = "swpcc@gmail.com"
            };
        }

		public SettingsStore()
		{
            _settingsPath = DocumentPath.For("SWPCCBilling.settings");
			_serializer = new DataContractJsonSerializer(typeof(Settings));
			_settings = null;
		}

		public Settings Load()
		{
			if (_settings != null)
				return _settings;

			if (File.Exists(_settingsPath))
			{
				using (var stream = new FileStream(_settingsPath, FileMode.Open))
					_settings = (Settings)_serializer.ReadObject(stream);
			} 
			else
			{
				_settings = DefaultSettings(); 
				Save(_settings);
			}

			return _settings;
		}

		public void Save(Settings settings)
		{
			_settings = settings;

			string settingsDir = Path.GetDirectoryName(_settingsPath);

			if (settingsDir == null)
				return;

			Directory.CreateDirectory(settingsDir);

			using (var stream = new FileStream(_settingsPath, FileMode.Create)) 
				_serializer.WriteObject(stream, settings);
		}
	}
}

