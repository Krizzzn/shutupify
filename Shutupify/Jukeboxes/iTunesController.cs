using iTunesLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shutupify.Jukeboxes
{
    public class iTunesController : IJukebox
    {
        private iTunesAppClass _itunes;
        private bool _wasPaused;
        private Dictionary<JukeboxCommand, string> _actionMapping;

        public iTunesController()
        {
            _actionMapping = new Dictionary<JukeboxCommand, string>();
            _actionMapping.Add(JukeboxCommand.Play, "Play");
            _actionMapping.Add(JukeboxCommand.Pause, "Pause");
            _actionMapping.Add(JukeboxCommand.NextTrack, "NextTrack");
            _actionMapping.Add(JukeboxCommand.PreviousTrack, "PreviousTrack");
            _actionMapping.Add(JukeboxCommand.Toggle, "PlayPause");
            _actionMapping.Add(JukeboxCommand.PlayAfterPaused, "Play");
        }

        public bool Active
        {
            get;
            set;
        }

        public void PerformAction(JukeboxCommand cmd)
        {
            if (_itunes == null) { 
                if (!IsItunesAvailable())
                    return;
                InitializeiTunes();
            }

            if (!_actionMapping.ContainsKey(cmd))
                return;

            if (cmd == JukeboxCommand.PlayAfterPaused && !_wasPaused)
                return;

            _wasPaused = false;
            var methodToCall = _actionMapping[cmd];

            if (cmd == JukeboxCommand.Pause && _itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying)
                _wasPaused = true;

            _itunes.GetType().GetMethod(methodToCall).Invoke(_itunes, new object[0]);
        }

        private void InitializeiTunes()
        {
            _itunes = new iTunesAppClass();

            _itunes.OnQuittingEvent += KillITunes;
            _itunes.OnAboutToPromptUserToQuitEvent += KillITunes;
        }

        private bool IsItunesAvailable()
        {
            return Process.GetProcesses()
                .Any(proc => proc.ProcessName.Equals("itunes", StringComparison.InvariantCultureIgnoreCase));
        }

        private void KillITunes()
        {
            if (_itunes == null)
                return;
            _itunes = null; 
            GC.Collect();
        }

        public string Name
        {
            get { return "iTunes"; }
        }
    }
}
