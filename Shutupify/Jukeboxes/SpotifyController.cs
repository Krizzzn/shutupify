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
            _actionMapping[JukeboxCommand.PlayAfterPaused] = PlayIfPaused;
            _actionMapping[JukeboxCommand.Pause] = PauseIfPlaying;
            _actionMapping[JukeboxCommand.Toggle] = () => Spotify.SendAction(SpotifyAction.PlayPause); 
        }

        public void PerformAction(JukeboxCommand cmd) {
            _actionMapping[cmd]();
        }

        private void PauseIfPlaying(){
            if (!Spotify.IsPlaying())
                return;
            Spotify.SendAction(SpotifyAction.PlayPause);
        }

        private void PlayIfPaused()
        {
            if (Spotify.IsPlaying())
                return;         
            Spotify.SendAction(SpotifyAction.PlayPause);
        }

        public bool IsAvailable
        {
            get {
                return Spotify.IsAvailable();
            }
        }

        public string Name
        {
            get { return "Spotify"; }
        }


        public bool IsPlaying
        {
            get { return Spotify.IsPlaying(); }
        }


        public bool IsActive
        {
            get;
            set;
        }
    }
}
