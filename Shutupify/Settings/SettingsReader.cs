using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shutupify.Settings
{
    public class SettingsReader : ISettingsReader
    {
        private List<string> _settings;
        private List<string> _keys;

        public SettingsReader(string settings)
        {
            this.Settings = settings;
        }

        private string ReadSingleLine(string line)
        {
            if (line.Contains("#"))
                line = line.Substring(0, line.IndexOf("#"));
            var lineSplit = line.Trim().Split(new[] { '=' }, 2);
            if (lineSplit.Length != 2 || line.StartsWith("#"))
                return"";
            return lineSplit[1].Trim();
        }

        public string SerializeToString() {
            return string.Join("\r\n", _settings.ToArray());
        }

        public string this [string key]
        {
            get
            {
                foreach (string line in _settings) {
                    if (Regex.IsMatch(line, @"^\W*"+key+@"\W*\=", RegexOptions.IgnoreCase))
                        return ReadSingleLine(line);
                }
                EnsureKey(key, null);
                return "";
            }
        }


        public void EnsureKey(string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            if (key.Contains('\n') || key.Contains('\r'))
                return;
            if (this.Keys.Contains(key))
                return;

            this._settings.Add("#" + key + " = " + defaultValue);
            ReadKeys();
        }

        public IEnumerable<string> Keys
        {
            get
            {
                if (_keys == null)
                    ReadKeys();
                return _keys.ToArray();
            }
        }

        private void ReadKeys()
        {
            _keys = this._settings
                .Where(line => !Regex.IsMatch(line, @"^( |#)*$"))
                .Select(line => (Regex.Match(line, @"^(\W|\#)*.*?(?=\W*\=)").Value ?? "").Trim())
                .Select(key => key.Replace("#","").Trim())
                .Where(key => !string.IsNullOrEmpty(key))
                .Distinct()
                .ToList();
        }


        public string Settings
        {
            set
            {
                _settings = new List<string>();

                foreach (var line in value.Split(new[] { '\n' }))
                    _settings.Add(line.Trim());
            }
            get {
                return SerializeToString();
            }
        }
    }
}
