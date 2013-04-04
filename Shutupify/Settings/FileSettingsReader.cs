using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify.Settings
{
    public class FileSettingsReader : ISettingsReader
    {
        private string settings;

        public FileSettingsReader(string settings)
        {
            Settings = new Dictionary<string, string>();

            foreach (var line in settings.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                ReadSingleLine(line);
            }

            this.settings = settings;
        }

        private void ReadSingleLine(string line)
        {
            if (line.Contains("#"))
                line = line.Substring(0, line.IndexOf("#"));
            var lineSplit = line.Trim().Split(new[] { '=' }, 2);
            if (lineSplit.Length != 2 || line.StartsWith("#"))
                return;
            this.Settings.Add(lineSplit[0].Trim().ToLower(), lineSplit[1].Trim());
        }

        public string SerializeToString() {
            return "";
        }

        public Dictionary<string, string> Settings
        {
            get;
            private set;
        }
    }
}
