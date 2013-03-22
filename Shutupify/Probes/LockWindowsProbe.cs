using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify.Probes
{
    public class LockWindowsProbe : IEventProbe
    {
        private bool _isActive;
        public LockWindowsProbe()
        {
            
        }

        public event Action<JukeboxCommand> ReactOnEvent;
        public bool Alive()
        {
            return _isActive;
        }

        public bool StartObserving()
        {
            if (_isActive)
                return false;

            Microsoft.Win32.SystemEvents.SessionSwitch += (s,e) => {
                if (ReactOnEvent == null)
                    return;

                switch (e.Reason) {
                    case Microsoft.Win32.SessionSwitchReason.ConsoleConnect:
                    case Microsoft.Win32.SessionSwitchReason.RemoteConnect:
                    case Microsoft.Win32.SessionSwitchReason.SessionRemoteControl:
                    case Microsoft.Win32.SessionSwitchReason.SessionLock:
                    case Microsoft.Win32.SessionSwitchReason.SessionLogoff:
                        ReactOnEvent(JukeboxCommand.Pause);
                        break;
                    case Microsoft.Win32.SessionSwitchReason.SessionLogon:
                    case Microsoft.Win32.SessionSwitchReason.SessionUnlock:
                        ReactOnEvent(JukeboxCommand.PlayAfterPaused);
                        break;
                    default:
                        break;
                }

            };
            _isActive = true;
            return true;
        }

    }
}
