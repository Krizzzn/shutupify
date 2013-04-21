
//// INSPIRED BY CODE from Sam Saffron https://github.com/SamSaffron (https://gist.github.com/SamSaffron/101357)
//// http://www.autoitscript.com/forum/topic/76230-vlc-automation-over-tcp/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Shutupify.Jukeboxes.Drivers {

    public class Vlcmote : IDisposable {
        DateTime _lastPlayingQuery;
        bool _lastAnswer;
        bool _clientCantBeFound;
        private TcpClient _client;

        public int Port { get; set; }

        public void Play()
        {
            if (IsPlaying())
                return;
            Toggle();
        }

        public void Pause()
        {
            if (!IsPlaying())
                return;
            Toggle();
        }

        public void Toggle() {
            SendCommand("pause", null);
            _lastPlayingQuery = DateTime.MinValue;
        }

        public void Next()
        {
            SendCommand("next", null);
            _lastPlayingQuery = DateTime.MinValue;
        }

        public void Previous()
        {
            SendCommand("prev", null);
            _lastPlayingQuery = DateTime.MinValue;
        }

        public bool IsPlaying() {
            if (DateTime.Now.Subtract(_lastPlayingQuery).TotalSeconds < 2)
                return _lastAnswer;
            var message = SendCommandAndReadResult("status", null);
            _lastPlayingQuery = DateTime.Now;
            _lastAnswer =  message.Contains("play state: 3");
            return _lastAnswer;
        }

        private TcpClient Client {
            get {
                if (_client != null && !_client.Connected) {
                    _client.Close();
                    _client = null;
                }

                if (_client == null)
                {
                    if (_clientCantBeFound)
                        return null;

                    try
                    {
                        _client = new TcpClient("localhost", this.Port);
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        _clientCantBeFound = true;
                    }
                }
                return _client;
            }
        }

        public string SendCommandAndReadResult(string command, string param)
        {
            SendCommand(command, param);
            Thread.Sleep(100);
            return ReadTillEnd();
        }

        void SendCommand(string command, string param)
        {
            if (Client == null)
                return;

            ReadTillEnd();

            string packet = command;
            if (param != null)
                packet += " " + param;

            packet += Environment.NewLine;
            
            var buffer = ASCIIEncoding.ASCII.GetBytes(packet);
            try
            {
                Client.GetStream().Write(buffer, 0, buffer.Length);
                Client.GetStream().Flush();
            }
            catch (System.IO.IOException)
            {
                Client.Close();
                _client = null;
            }
        }

        private string ReadTillEnd()
        {
            if (Client == null)
                return "";

            StringBuilder sb = new StringBuilder();
            while (Client.GetStream().DataAvailable)
            {
                int b = Client.GetStream().ReadByte();
                if (b >= 0)
                {
                    sb.Append((char)b);
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_client != null)
                    {
                        SendCommand("logout", "");
                        Thread.Sleep(100);
                        _client.Close();
                        _client = null;
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

        ~Vlcmote() // the finalizer
        {
            Dispose(false);
        }
        #endregion
    }
}
