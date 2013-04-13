using Shutupify.Jukeboxes.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Shutupify.Jukeboxes
{
    public class WebSocketController : IJukebox, IDisposable
    {
        WebSocket _socket;
        Dictionary<JukeboxCommand, string> _messageMapping;

        public WebSocketController() : this(new WebSocket())
        {}

        public WebSocketController(WebSocket socket)
        {
            _socket = socket;

            _messageMapping = new Dictionary<JukeboxCommand, string>();
            _messageMapping.Add(JukeboxCommand.Play, "PLAY!");
            _messageMapping.Add(JukeboxCommand.Pause, "PAUSE!");
            //_messageMapping.Add(JukeboxCommand.NextTrack, "NextTrack");
            //_messageMapping.Add(JukeboxCommand.PreviousTrack, "PreviousTrack");
            //_messageMapping.Add(JukeboxCommand.Toggle, "PlayPause");
            _messageMapping.Add(JukeboxCommand.PlayAfterPaused, "PLAY!");

            this.IsAvailable = true;
        }

        public bool IsAvailable
        {
            get;
            set;
        }

        public void PerformAction(JukeboxCommand cmd)
        {
            if (!_messageMapping.ContainsKey(cmd))
                return;

            _socket.SendMessage(_messageMapping[cmd]);
        }

        public string Name
        {
            get { return "WebSocket"; }
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_socket != null)
                        _socket.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose() // Implement IDisposable
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebSocketController() // the finalizer
        {
            Dispose(false);
        }
        #endregion


        public bool IsPlaying
        {
            get { throw new NotImplementedException(); }
        }


        public bool IsActive
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
