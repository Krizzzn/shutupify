using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify.Settings
{
    public interface ISettable
    {
        void ReadSettings(ISettingsReader settings);
    }
}
