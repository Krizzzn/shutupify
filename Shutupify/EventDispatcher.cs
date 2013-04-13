using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shutupify
{
    public class EventDispatcher
    {
        private bool _wasPaused;
        private IJukebox[] _jukeboxes;
        private IJukebox _lastJukebox;

        public EventDispatcher(IJukebox[] jukeboxes)
        {
            this._jukeboxes = jukeboxes;
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

        private IJukebox GetCurrentPlayer()
        {
            var player = _jukeboxes.Where(p => p.IsPlaying && p.IsActive).FirstOrDefault();
            if (player == null)
                player = _lastJukebox;
            if (player == null && _jukeboxes.Where(p => p.IsActive).Count() == 1)
                player = _jukeboxes.Where(p => p.IsActive).Single();

            if (player != _lastJukebox)
                _wasPaused = false;
            return player;
        }
    }
}
