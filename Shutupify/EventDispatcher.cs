using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify
{
    public class EventDispatcher : IEventDispatcher
    {
        private bool _wasPaused;
        private IJukebox _lastJukebox;

        public EventDispatcher()
        {

        }

        public EventDispatcher(IJukebox[] jukeboxes)
        {
            this.Jukeboxes = jukeboxes;
        }

        public void Dispatch(JukeboxCommand cmd)
        {
            var player = GetCurrentPlayer();

            if (player == null)
                return;

            if (cmd == JukeboxCommand.PlayAfterPaused) {
                if (_wasPaused)
                    cmd = JukeboxCommand.Play;
                else
                    return;
            }

            _wasPaused = (cmd == JukeboxCommand.Pause && player.IsPlaying);

            player.PerformAction(cmd);
            _lastJukebox = player;
        }

        public IEnumerable<IJukebox> Jukeboxes { get; set; }

        private IJukebox GetCurrentPlayer()
        {
            var player = Jukeboxes.Where(p => p.IsActive && p.IsAvailable && p.IsPlaying).FirstOrDefault();
            if (player == null)
                player = _lastJukebox;
            if (player == null && Jukeboxes.Where(p => p.IsActive && p.IsAvailable).Count() == 1)
                player = Jukeboxes.Where(p => p.IsAvailable && p.IsAvailable).Single();

            if (player != _lastJukebox)
                _wasPaused = false;
            return player;
        }
    }
}
