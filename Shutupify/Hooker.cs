using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shutupify.Jukeboxes;
using Shutupify.Probes;

namespace Shutupify
{
    public class Hooker
    {
        public Hooker()
        {

        }

        public void Hookup() {
            _spotify = new SpotifyController();

            List<IEventProbe> probes = new List<IEventProbe>();
            probes.Add(new LyncCallProbe());
            probes.Add(new HotkeysProbe());
            probes.Add(new LockWindowsProbe());

            probes.ForEach(probe => {
                probe.ReactOnEvent += _spotify.PerformAction;
                probe.ReactOnEvent += BubbleReactOnEvent;
                probe.StartObserving();
            });
        }

        private void BubbleReactOnEvent(JukeboxCommand obj)
        {
            if (this.ReactOnEvent != null)
                this.ReactOnEvent(obj);
        }

        public event Action<JukeboxCommand> ReactOnEvent;

        SpotifyController _spotify;
    }
}
