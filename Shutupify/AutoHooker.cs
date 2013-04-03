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

        }

        public void Hookup() {
            _probes = FindClassesAndCreateInstances<IEventProbe>(Assembly.GetExecutingAssembly());
            _jukeboxes = FindClassesAndCreateInstances<IJukebox>(Assembly.GetExecutingAssembly());

            _probes.ForEach(probe => {
                _jukeboxes.ForEach(jukebox => probe.ReactOnEvent += jukebox.PerformAction);
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
    }
}
