using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Shutupify.Settings;

namespace Shutupify
{
    public class AutoHooker
    {
        List<IEventProbe> _probes;
        List<IJukebox> _jukeboxes;
        IEventDispatcher _dispatcher;

        public AutoHooker() : this(null, null)
        {}

        public AutoHooker(Settings.ISettingsReader settings, IEventDispatcher dispatcher)
        {
            _probes = new List<IEventProbe>();
            _jukeboxes = new List<IJukebox>();
            LoadFromAssembly(Assembly.GetExecutingAssembly());
            this._settings = settings;
            this._dispatcher = dispatcher ?? new EventDispatcher();
            _dispatcher.Jukeboxes = _jukeboxes;
        }

        public IEventProbe[] Probes { get { return _probes.ToArray(); } }
        public IJukebox[] Jukeboxes { get { return _jukeboxes.ToArray(); } }
        public event Action<JukeboxCommand> ReactOnEvent;
        private ISettingsReader _settings;

        public void Hookup() {
            Setup(_probes);
            Setup(_jukeboxes);

            _probes.ForEach(probe => {
                probe.ReactOnEvent += _dispatcher.Dispatch;
                probe.ReactOnEvent += BubbleReactOnEvent;

                if (IsActive(probe))
                    probe.StartObserving();
            });

            _jukeboxes.ForEach(jukebox => jukebox.IsActive = IsActive(jukebox));
        }

        private bool IsActive<T>(T item) where T : IName
        {
            if (_settings == null)
                return true;

            var validValues = new[]{"yes", "true", "1", "active", "on"};
            return (validValues.Contains((_settings[item.Name + ":activated"]??"").ToLower()));
        }

        private void Setup<T>(List<T> list) where T : IName
        {
            if (_settings == null)
                return;

            list.ForEach((item) => {
                _settings.EnsureKey(item.Name + ":activated", "no");

                if (item is ISettable)
                    ((ISettable)item).ReadSettings(_settings);
            });
        }

        public void LoadFromAssembly(Assembly asm) {
            _probes.AddRange(FindClassesAndCreateInstances<IEventProbe>(asm));
            _jukeboxes.AddRange(FindClassesAndCreateInstances<IJukebox>(asm));
        }

        private void BubbleReactOnEvent(JukeboxCommand obj)
        {
            if (this.ReactOnEvent != null)
                this.ReactOnEvent(obj);
        }

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
