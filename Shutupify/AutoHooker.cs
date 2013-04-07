using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Shutupify
{
    public class AutoHooker
    {
        List<IEventProbe> _probes;
        List<IJukebox> _jukeboxes;

        public AutoHooker()
        {
            _probes = new List<IEventProbe>();
            _jukeboxes = new List<IJukebox>();
            LoadFromAssembly(Assembly.GetExecutingAssembly());
        }

        public void Hookup() {
            _probes.ForEach(probe => {
                _jukeboxes.ForEach(jukebox => probe.ReactOnEvent += jukebox.PerformAction);
                probe.ReactOnEvent += BubbleReactOnEvent;
                probe.StartObserving();
            });
        }

        public IEventProbe[] Probes { get { return _probes.ToArray(); } }
        public IJukebox[] Jukeboxes { get { return _jukeboxes.ToArray(); } }

        public void LoadFromAssembly(Assembly asm) {
            _probes.AddRange(FindClassesAndCreateInstances<IEventProbe>(asm));
            _jukeboxes.AddRange(FindClassesAndCreateInstances<IJukebox>(asm));
        }

        private void BubbleReactOnEvent(JukeboxCommand obj)
        {
            if (this.ReactOnEvent != null)
                this.ReactOnEvent(obj);
        }

        public event Action<JukeboxCommand> ReactOnEvent;

        private List<T> FindClassesAndCreateInstances<T>(Assembly asm) {
            var types = asm.GetTypes().Where(m => m.GetInterfaces().Contains(typeof(T))).ToArray();
            
            var probes = types.Select((type) => {
                var ctor = type.GetConstructor(new Type[0]);
                if (ctor != null)
                    return (T)ctor.Invoke(new object[0]);
                return default(T);
            })
            .Where(m => m!=null)
            .ToList();

            return probes;
        }

        public void Clear()
        {
            _probes.ForEach(probe => { if (probe is IDisposable) ((IDisposable)probe).Dispose(); });
            _jukeboxes.ForEach(jukebox => { if (jukebox is IDisposable) ((IDisposable)jukebox).Dispose(); });

            _jukeboxes.Clear();
            _probes.Clear();
        }

        public void Add(IJukebox jukebox)
        {
            _jukeboxes.Add(jukebox);
        }

        public void Add(IEventProbe eventProbe)
        {
            _probes.Add(eventProbe);
        }
    }
}
