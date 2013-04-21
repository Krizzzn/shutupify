using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify.Settings
{
    public class FileReader : ISettingsReader
    {
        public FileReader(string fileName) : this(fileName, new SettingsReader(""))
        {

        }

        public FileReader(string fileName, ISettingsReader settings)
        {
            if (settings is FileReader)
                throw new ArgumentException("settings cannot be of type FileReader", "settings");
            
            var settingsText = "";
            if (System.IO.File.Exists(fileName))
                settingsText = System.IO.File.ReadAllText(fileName);
            settings.Settings = settingsText;
            _settings = settings;
            FileName = fileName;
        }

        public string this[string key]
        {
            get { return _settings[key]; }
        }

        public IEnumerable<string> Keys
        {
            get { return _settings.Keys; }
        }

        public void EnsureKey(string key, string defaultValue)
        {
            _settings.EnsureKey(key, defaultValue);
        }

        public string Settings
        {
            set { _settings.Settings = value; }
            get { return _settings.Settings; }
        }

        private ISettingsReader _settings;

        public void Save()
        {
            System.IO.File.WriteAllText(FileName, this.Settings);
        }

        public string FileName { get; private set; }
    }
}
