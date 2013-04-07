using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shutupify.Jukeboxes.Drivers;

namespace Shutupify.Jukeboxes
{
    public class SpotifyController : Shutupify.IJukebox
    {
        private Dictionary<JukeboxCommand, Action> _actionMapping;
        private bool _wasPaused;

        public SpotifyController()
        {
            InitActionMapping();
        }

        private void InitActionMapping()
        {
            _actionMapping = new Dictionary<JukeboxCommand, Action>();
            foreach (var item in Enum.GetNames(typeof(JukeboxCommand)))
                _actionMapping.Add((JukeboxCommand)Enum.Parse(typeof(JukeboxCommand), item), () => { });

            _actionMapping[JukeboxCommand.NextTrack] = () => Spotify.SendAction(SpotifyAction.NextTrack);
            _actionMapping[JukeboxCommand.PreviousTrack] = () => Spotify.SendAction(SpotifyAction.PreviousTrack);
            _actionMapping[JukeboxCommand.Play] = PlayIfPaused;
            _actionMapping[JukeboxCommand.PlayAfterPaused] = () => { if (_wasPaused) PlayIfPaused(); };
            _actionMapping[JukeboxCommand.Pause] = PauseIfPlaying;
            _actionMapping[JukeboxCommand.Toggle] = () => { _wasPaused = false; Spotify.SendAction(SpotifyAction.PlayPause); };
        }

        public void PerformAction(JukeboxCommand cmd) {
            _actionMapping[cmd]();
        }

        private void PauseIfPlaying(){
            if (!Spotify.IsPlaying()) {
                _wasPaused = false;
                return;
            }
            Spotify.SendAction(SpotifyAction.PlayPause);
            _wasPaused = true;
        }

        private void PlayIfPaused()
        {
            if (Spotify.IsPlaying())
                return;
            
            Spotify.SendAction(SpotifyAction.PlayPause);
            _wasPaused = false;
        }

        public bool Active
        {
            get;
            set;
        }


        public string Name
        {
            get { return "Spotify"; }
        }
    }
}
