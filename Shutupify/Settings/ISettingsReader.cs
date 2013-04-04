using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify.Settings
{
    public interface ISettingsReader
    {
        Dictionary<string, string> Settings { get; }
    }
}
