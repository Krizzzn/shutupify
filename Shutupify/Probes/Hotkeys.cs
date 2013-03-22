using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedWinapi;

namespace Shutupify.Probes
{
    public class HotkeysProbe : IEventProbe
    {
        List<ManagedWinapi.Hotkey> _hotkeys;

        public HotkeysProbe()
        {
            _hotkeys = new List<Hotkey>();
        }

        ~HotkeysProbe()
        {
            while (_hotkeys.Count > 0)
            {
                if (_hotkeys[0] != null) {
                    _hotkeys[0].Enabled = false;
                    _hotkeys[0].Dispose();
                }
                _hotkeys.RemoveAt(0);
            }

        }

        public event Action<JukeboxCommand> ReactOnEvent;

        public bool Alive()
        {
            return (_hotkeys != null &&_hotkeys.Count > 0 && _hotkeys[0].Enabled);
        }

        public bool StartObserving()
        {
            if (Alive())
                return false;

           new[] { 
                new { Key = System.Windows.Forms.Keys.Up, Action = JukeboxCommand.Play} ,
                new { Key = System.Windows.Forms.Keys.Down, Action = JukeboxCommand.Pause} ,
                new { Key = System.Windows.Forms.Keys.Left, Action = JukeboxCommand.PreviousTrack} ,
                new { Key = System.Windows.Forms.Keys.Right, Action = JukeboxCommand.NextTrack} 
            }.ToList()
            .ForEach((hot) => {
                RegisterHotKey(hot.Key, hot.Action);
            });

           _hotkeys.ForEach(hot => { hot.Enabled = true; });

            return true;
        }

        private void RegisterHotKey(System.Windows.Forms.Keys keys, JukeboxCommand spotifyCommand)
        {
            var hotkey = new ManagedWinapi.Hotkey();

            if (hotkey.Enabled)
                hotkey.Enabled = false;

            hotkey.Alt = true;
            hotkey.Ctrl = true;
            hotkey.Shift = true;
            hotkey.KeyCode = keys;

            hotkey.HotkeyPressed += (s, e) => {
                if (ReactOnEvent != null)
                    ReactOnEvent(spotifyCommand);
            };

            this._hotkeys.Add(hotkey);
        }
    }
}
