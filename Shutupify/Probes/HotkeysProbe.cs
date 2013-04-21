using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedWinapi;
using Shutupify.Settings;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Shutupify.Probes
{
    public class HotkeysProbe : IEventProbe, IDisposable, ISettable
    {
        List<ManagedWinapi.Hotkey> _hotkeys;
        private bool _isDisposed;

        public HotkeysProbe()
        {
            _hotkeys = new List<Hotkey>();
        }

        ~HotkeysProbe()
        {
            this.Dispose();
        }

        public event Action<JukeboxCommand> ReactOnEvent;

        public bool Alive()
        {
            return (_hotkeys != null &&_hotkeys.Count > 0 && _hotkeys[0].Enabled);
        }

        public bool StartObserving()
        {
            if (_hotkeys == null)
                RegisterDefaultHotKeys();

            if (Alive())
                return false;

           _hotkeys.ForEach(hot => { hot.Enabled = true; });

            return true;
        }

        private void RegisterDefaultHotKeys()
        {
            RegisterHotKey("CTRL+ALT+SHIFT+Up", JukeboxCommand.Play);
            RegisterHotKey("CTRL+ALT+SHIFT+Down", JukeboxCommand.Pause);
            RegisterHotKey("CTRL+ALT+SHIFT+Left", JukeboxCommand.PreviousTrack);
            RegisterHotKey("CTRL+ALT+SHIFT+Right", JukeboxCommand.NextTrack);
        }

        public IEnumerable<ManagedWinapi.Hotkey> Hotkeys {
            get {
                return _hotkeys.ToArray();
            }
        }

        private void RegisterHotKey(string hotkeyCombination, JukeboxCommand spotifyCommand)
        {
            var k = ParseKeyString(hotkeyCombination);
            if (k == Keys.None)
                return;

            var hotkey = new ManagedWinapi.Hotkey();
            if (hotkey.Enabled)
                hotkey.Enabled = false;

            hotkey.Alt = hotkeyCombination.Contains("ALT");
            hotkey.Ctrl = hotkeyCombination.Contains("CTRL") || hotkeyCombination.Contains("STRG"); 
            hotkey.Shift = hotkeyCombination.Contains("SHIFT");

            if (!hotkey.Alt && !hotkey.Ctrl && !hotkey.Shift) {
                hotkey.Dispose();
                return;
            }

            hotkey.KeyCode = k;

            hotkey.HotkeyPressed += (s, e) => {
                if (ReactOnEvent != null)
                    ReactOnEvent(spotifyCommand);
            };
            this._hotkeys.Add(hotkey);
        }

        private Keys ParseKeyString(string hotkeyCombination)
        {
            var invalid = new[] { Keys.Alt, Keys.Control, Keys.ControlKey, Keys.Shift, Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey, Keys.LControlKey, Keys.RControlKey };
            hotkeyCombination = hotkeyCombination.ToUpper();
            var value = Regex.Match(hotkeyCombination, @"[A-z0-9]+$").Value;
            Keys k = Keys.None;
            if (!Enum.TryParse<Keys>(value, true, out k))
                return Keys.None;
            if (invalid.Contains(k))
                return Keys.None;
            return k;
        }

        public string Name
        {
            get { return "Hotkeys"; }
        }

        public void Dispose()
        {
            if (this._isDisposed) return;

            while (_hotkeys.Count > 0)
            {
                if (_hotkeys[0] != null)
                {
                    _hotkeys[0].Enabled = false;
                    _hotkeys[0].Dispose();
                }
                _hotkeys.RemoveAt(0);
            }
            _isDisposed = true;
        }

        public void ReadSettings(ISettingsReader settings)
        {
            settings.EnsureKey(this.Name + ":" + JukeboxCommand.Play.ToString(), "CTRL+ALT+SHIFT+Up");
            settings.EnsureKey(this.Name + ":" + JukeboxCommand.Pause.ToString(), "CTRL+ALT+SHIFT+Down");
            settings.EnsureKey(this.Name + ":" + JukeboxCommand.PreviousTrack.ToString(), "CTRL+ALT+SHIFT+Left");
            settings.EnsureKey(this.Name + ":" + JukeboxCommand.NextTrack.ToString(), "CTRL+ALT+SHIFT+Right");

            foreach (string commandName in Enum.GetNames(typeof(JukeboxCommand))) {
                if (commandName == "PlayAfterPause")
                    continue;
                var keys = settings[this.Name + ":" + commandName];
                if (string.IsNullOrEmpty(keys))
                    continue;
                var command = (JukeboxCommand)Enum.Parse(typeof(JukeboxCommand), commandName);
                RegisterHotKey(keys, command);
            }
        }
    }
}
