using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify.Settings
{
    public interface ISettingsReader
    {
        string this[string key]{get;}
        IEnumerable<string> Keys { get; }
        void EnsureKey(string key, string defaultValue);

        string Settings { set; get; }
    }
}
