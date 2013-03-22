using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify
{
    public interface IEventProbe
    {
        event Action<JukeboxCommand> ReactOnEvent;

        bool Alive();

        bool StartObserving();
    }
}
