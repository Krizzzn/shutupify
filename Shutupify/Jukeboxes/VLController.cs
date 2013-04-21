using Shutupify.Jukeboxes.Drivers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shutupify.Jukeboxes
{
    public class VLController : IJukebox, IDisposable
    {
        Vlcmote _remote;
        Dictionary<JukeboxCommand, string> _actionMapping;

        public VLController()
        {
            _remote = new Vlcmote();

            _actionMapping = new Dictionary<JukeboxCommand, string>();
            _actionMapping.Add(JukeboxCommand.Play, "Play");
            _actionMapping.Add(JukeboxCommand.Pause, "Pause");
            _actionMapping.Add(JukeboxCommand.NextTrack, "Next");
            _actionMapping.Add(JukeboxCommand.PreviousTrack, "Previous");
            _actionMapping.Add(JukeboxCommand.Toggle, "Toggle");
            _actionMapping.Add(JukeboxCommand.PlayAfterPaused, "Play");
        }

        public void PerformAction(JukeboxCommand cmd)
        {
            if (_remote == null)
                return;

            if (!_actionMapping.ContainsKey(cmd))
                return;

            var methodToCall = _actionMapping[cmd];
            _remote.GetType().GetMethod(methodToCall).Invoke(_remote, new object[0]);
        }

        public bool IsActive
        {
            get
            {
                return true;
            }
            set{
            
            }
        }

        public bool IsAvailable
        {
            get {
                var avail = Process.GetProcesses()
                    .Any(proc => proc.ProcessName.Equals("vlc", StringComparison.InvariantCultureIgnoreCase));
                if (!avail && _remote != null)
                {
                    _remote.Dispose();
                    _remote = null;
                }
                return avail;
            }
        }

        public bool IsPlaying
        {
            get { return _remote.IsPlaying(); }
        }

        public string Name
        {
            get { return "VLC"; }
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_remote != null)
                    {
                        _remote.Dispose();
                        _remote = null;
                    }
                }
                disposed = true;
            }
        }

        public void Dispose() // Implement IDisposable
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VLController() // the finalizer
        {
            Dispose(false);
        }
        #endregion
    }
}
