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

        public void PerformAction(JukeboxCommand cmd)
        {
            if (iTunes == null)
                return;
            var itunes = this.iTunes;
            if (!_actionMapping.ContainsKey(cmd))
                return;

            var methodToCall = _actionMapping[cmd];
            itunes.GetType().GetMethod(methodToCall).Invoke(itunes, new object[0]);
        }

        private iTunesAppClass iTunes
        {
            get
            {
                if (_itunes == null)
                {
                    if (!(IsActive && IsAvailable))
                        return null;
                    InitializeiTunes();
                }
                return _itunes;
            }
        }

        private void InitializeiTunes()
        {
            _itunes = new iTunesAppClass();

            _itunes.OnQuittingEvent += KillITunes;
            _itunes.OnAboutToPromptUserToQuitEvent += KillITunes;
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


        public bool IsPlaying
        {
            get {
                if (iTunes == null)
                    return false;
                return iTunes.PlayerState == ITPlayerState.ITPlayerStatePlaying;
            }
        }


        public bool IsActive
        {
            get;
            set;
        }

        public bool IsAvailable
        {
            get {
                return Process.GetProcesses()
                    .Any(proc => proc.ProcessName.Equals("itunes", StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
